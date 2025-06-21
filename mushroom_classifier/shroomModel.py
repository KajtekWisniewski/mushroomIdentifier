import os
import random

import cv2
import kagglehub
import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
import seaborn as sns
import tensorflow as tf
from keras.callbacks import EarlyStopping, ReduceLROnPlateau
from keras.layers import Conv2D, Dense, Dropout, Flatten, Input, MaxPooling2D
from PIL import ImageFile
from sklearn.metrics import classification_report, confusion_matrix
from sklearn.model_selection import train_test_split
from sklearn.utils.class_weight import compute_class_weight
from tensorflow.keras.preprocessing.image import ImageDataGenerator


def make_gradcam_heatmap(img_array, model, last_conv_layer_name, pred_index=None):
    grad_model = tf.keras.models.Model(
        model.inputs, [model.get_layer(last_conv_layer_name).output, model.output]
    )
    with tf.GradientTape() as tape:
        conv_outputs, predictions = grad_model(img_array)
        if pred_index is None:
            pred_index = tf.argmax(predictions[0])
        class_channel = predictions[:, pred_index]

    grads = tape.gradient(class_channel, conv_outputs)
    pooled_grads = tf.reduce_mean(grads, axis=(0, 1, 2))
    conv_outputs = conv_outputs[0]
    heatmap = conv_outputs @ pooled_grads[..., tf.newaxis]
    heatmap = tf.squeeze(heatmap)
    heatmap = tf.maximum(heatmap, 0) / tf.math.reduce_max(heatmap)
    return heatmap.numpy()


def advanced_preprocessing(image):
    image = tf.image.random_contrast(image, lower=0.8, upper=1.2)
    image = tf.image.random_brightness(image, max_delta=0.1)
    return image


print("Downloading dataset from Kaggle...")
data_dir = kagglehub.dataset_download(
    "maysee/mushrooms-classification-common-genuss-images"
)

mushrooms_dir = os.path.join(data_dir, "Mushrooms")

print(data_dir)

# Define image parameters
img_height, img_width = 224, 244
batch_size = 32

# Enable loading of truncated images
ImageFile.LOAD_TRUNCATED_IMAGES = True

# Prepare dataframe for stratified split
print("Preparing dataframe...")
all_image_paths = []
all_labels = []

for class_name in sorted(os.listdir(mushrooms_dir)):
    class_dir = os.path.join(mushrooms_dir, class_name)
    if os.path.isdir(class_dir):
        for fname in os.listdir(class_dir):
            if fname.lower().endswith((".png", ".jpg", ".jpeg")):
                all_image_paths.append(os.path.join(class_dir, fname))
                all_labels.append(class_name)

df = pd.DataFrame({"filepath": all_image_paths, "label": all_labels})

# Stratified split
train_df, val_df = train_test_split(
    df, test_size=0.2, stratify=df["label"], random_state=42
)

print(f"Train size: {len(train_df)}, Validation size: {len(val_df)}")

# Data generators
train_datagen = ImageDataGenerator(
    rescale=1.0 / 255,
    rotation_range=15,
    width_shift_range=0.05,
    height_shift_range=0.05,
    shear_range=0.05,
    zoom_range=0.1,
    horizontal_flip=True,
    brightness_range=[0.8, 1.2],
    fill_mode="nearest",
    preprocessing_function=advanced_preprocessing,
)

val_datagen = ImageDataGenerator(rescale=1.0 / 255)

# Flow from dataframe
train_data = train_datagen.flow_from_dataframe(
    train_df,
    x_col="filepath",
    y_col="label",
    target_size=(img_height, img_width),
    batch_size=batch_size,
    class_mode="categorical",
    shuffle=True,
)

validation_data = val_datagen.flow_from_dataframe(
    val_df,
    x_col="filepath",
    y_col="label",
    target_size=(img_height, img_width),
    batch_size=batch_size,
    class_mode="categorical",
    shuffle=False,
)

# Compute class weights
print("Computing class weights...")
class_indices = train_data.class_indices
inverse_class_indices = {v: k for k, v in class_indices.items()}
y_train_labels = train_data.classes

class_weights_values = compute_class_weight(
    class_weight="balanced",
    classes=np.unique(y_train_labels),
    y=y_train_labels,
)
class_weights = dict(enumerate(class_weights_values))

print("Class weights:", class_weights)

# Build CNN model
print("Building CNN model...")
inputs = Input(shape=(img_height, img_width, 3))

x = Conv2D(32, (3, 3), activation="relu")(inputs)
x = MaxPooling2D(2, 2)(x)

x = Conv2D(64, (3, 3), activation="relu")(x)
x = MaxPooling2D(2, 2)(x)

x = Conv2D(128, (3, 3), activation="relu")(x)
x = MaxPooling2D(2, 2)(x)

x = Conv2D(256, (3, 3), activation="relu")(x)
x = MaxPooling2D(2, 2)(x)

x = Conv2D(256, (3, 3), activation="relu", name="last_conv")(x)
x = MaxPooling2D(2, 2)(x)

x = Flatten()(x)
x = Dense(256, activation="relu")(x)
x = Dropout(0.4)(x)
outputs = Dense(len(train_data.class_indices), activation="softmax")(x)

model = tf.keras.models.Model(inputs, outputs)

# Compile the model
model.compile(optimizer="adam", loss="categorical_crossentropy", metrics=["accuracy"])

early_stop = EarlyStopping(monitor="val_loss", patience=5, restore_best_weights=True)
lr_scheduler = ReduceLROnPlateau(monitor="val_loss", factor=0.5, patience=3, verbose=1)

# Train the model with class weights
print("Training model with class weights...")
history = model.fit(
    train_data,
    validation_data=validation_data,
    epochs=30,
    class_weight=class_weights,
    callbacks=[early_stop, lr_scheduler],
)

# Save the model
model_save_path = "aiModels/mushroom_classifier_model_20epochs.h5"
model.save(model_save_path)
print(f"Model saved to {model_save_path}")

# Evaluate the model
print("Evaluating model...")
evaluation = model.evaluate(validation_data)
print(f"Validation Loss: {evaluation[0]}, Validation Accuracy: {evaluation[1]}")

os.makedirs("plots", exist_ok=True)

true_classes = validation_data.classes
class_labels = list(validation_data.class_indices.keys())

last_conv_layer_name = "last_conv"
os.makedirs("plots/gradcam_examples", exist_ok=True)

for class_name in class_labels:
    print(f"Generating Grad-CAMs for class: {class_name}")
    class_dir = os.path.join(mushrooms_dir, class_name)
    image_files = os.listdir(class_dir)
    selected_files = random.sample(image_files, 2)

    for idx, img_file in enumerate(selected_files):
        img_path = os.path.join(class_dir, img_file)

        # Load image
        img = tf.keras.preprocessing.image.load_img(
            img_path, target_size=(img_height, img_width)
        )
        img_array = tf.keras.preprocessing.image.img_to_array(img)
        img_array = np.expand_dims(img_array, axis=0)
        img_array /= 255.0

        # Compute heatmap
        heatmap = make_gradcam_heatmap(img_array, model, last_conv_layer_name)

        # Superimpose on original image
        img_orig = cv2.imread(img_path)
        img_orig = cv2.resize(img_orig, (img_width, img_height))
        heatmap = cv2.resize(heatmap, (img_width, img_height))
        heatmap = np.uint8(255 * heatmap)
        heatmap_color = cv2.applyColorMap(heatmap, cv2.COLORMAP_JET)
        superimposed_img = cv2.addWeighted(img_orig, 0.6, heatmap_color, 0.4, 0)

        # Save
        save_path = f"plots/gradcam_examples/{class_name}_{idx + 1}.png"
        cv2.imwrite(save_path, superimposed_img)
        print(f"Saved: {save_path}")

print("Grad-CAM visualization DONE. Check folder 'plots/gradcam_examples/'.")

# Accuracy plot
plt.figure(figsize=(8, 6))
plt.plot(history.history["accuracy"], label="Train Accuracy")
plt.plot(history.history["val_accuracy"], label="Validation Accuracy")
plt.title("Model Accuracy")
plt.xlabel("Epoch")
plt.ylabel("Accuracy")
plt.legend()
plt.grid(True)
plt.savefig("plots/accuracy_curve.png")
plt.show()

# Loss plot
plt.figure(figsize=(8, 6))
plt.plot(history.history["loss"], label="Train Loss")
plt.plot(history.history["val_loss"], label="Validation Loss")
plt.title("Model Loss")
plt.xlabel("Epoch")
plt.ylabel("Loss")
plt.legend()
plt.grid(True)
plt.savefig("plots/loss_curve.png")
plt.show()

print("Generating confusion matrix and classification report...")

# Predict on validation set
Y_pred = model.predict(validation_data)
y_pred = np.argmax(Y_pred, axis=1)

# Confusion Matrix
cm = confusion_matrix(true_classes, y_pred)

plt.figure(figsize=(10, 8))
sns.heatmap(
    cm,
    annot=True,
    fmt="d",
    cmap="Blues",
    xticklabels=class_labels,
    yticklabels=class_labels,
)
plt.title("Confusion Matrix")
plt.ylabel("True Label")
plt.xlabel("Predicted Label")
plt.savefig("plots/confusion_matrix.png")
plt.show()

# Classification Report
report = classification_report(true_classes, y_pred, target_names=class_labels)
print(report)

with open("plots/classification_report.txt", "w") as f:
    f.write(report)

print("Done! All plots saved to 'plots/' folder.")
