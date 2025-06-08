import os

import kagglehub
import matplotlib.pyplot as plt
import numpy as np
import seaborn as sns
import tensorflow as tf
from keras.callbacks import EarlyStopping, ReduceLROnPlateau
from keras.layers import Conv2D, Dense, Dropout, Flatten, MaxPooling2D
from keras.models import Sequential
from PIL import ImageFile
from sklearn.metrics import classification_report, confusion_matrix
from tensorflow.keras.preprocessing.image import ImageDataGenerator

print("Downloading dataset from Kaggle...")
data_dir = kagglehub.dataset_download(
    "maysee/mushrooms-classification-common-genuss-images"
)

mushrooms_dir = os.path.join(data_dir, "Mushrooms")

print(data_dir)
# Define image parameters
img_height, img_width = 150, 150
batch_size = 32

# Create train-validation split

ImageFile.LOAD_TRUNCATED_IMAGES = True

print("Preparing dataset...")
data_gen = ImageDataGenerator(
    rescale=1.0 / 255,
    validation_split=0.2,
    rotation_range=15,
    width_shift_range=0.05,
    height_shift_range=0.05,
    shear_range=0.05,
    zoom_range=0.1,
    horizontal_flip=True,
    fill_mode="nearest",
)


train_data = data_gen.flow_from_directory(
    mushrooms_dir,
    target_size=(img_height, img_width),
    batch_size=batch_size,
    class_mode="categorical",
    subset="training",
    shuffle=True,
)

print(train_data.class_indices)

validation_data = data_gen.flow_from_directory(
    mushrooms_dir,
    target_size=(img_height, img_width),
    batch_size=batch_size,
    class_mode="categorical",
    subset="validation",
)

# Build CNN model
print("Building CNN model...")
model = Sequential(
    [
        Conv2D(32, (3, 3), activation="relu", input_shape=(img_height, img_width, 3)),
        MaxPooling2D(2, 2),
        Conv2D(64, (3, 3), activation="relu"),
        MaxPooling2D(2, 2),
        Conv2D(128, (3, 3), activation="relu"),
        MaxPooling2D(2, 2),
        Conv2D(256, (3, 3), activation="relu"),
        MaxPooling2D(2, 2),
        Flatten(),
        Dense(256, activation="relu"),
        Dropout(0.5),
        Dense(len(train_data.class_indices), activation="softmax"),
    ]
)

# Compile the model
model.compile(optimizer="adam", loss="categorical_crossentropy", metrics=["accuracy"])

early_stop = EarlyStopping(monitor="val_loss", patience=5, restore_best_weights=True)
lr_scheduler = ReduceLROnPlateau(monitor="val_loss", factor=0.5, patience=3, verbose=1)

# Train the model
print("Training model...")
history = model.fit(train_data, validation_data=validation_data, epochs=30)

# Save the model
model_save_path = "aiModels/mushroom_classifier_model_20epochs.h5"
model.save(model_save_path)
print(f"Model saved to {model_save_path}")

# Evaluate the model
print("Evaluating model...")
evaluation = model.evaluate(validation_data)
print(f"Validation Loss: {evaluation[0]}, Validation Accuracy: {evaluation[1]}")

# --- 🔟 PLOTS for paper ---
os.makedirs("plots", exist_ok=True)

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

# --- 1️⃣1️⃣ Confusion Matrix + Classification Report ---
print("Generating confusion matrix and classification report...")

# Predict on validation set
Y_pred = model.predict(validation_data)
y_pred = np.argmax(Y_pred, axis=1)

# True labels
true_classes = validation_data.classes
class_labels = list(validation_data.class_indices.keys())

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

# Optionally save report to file
with open("plots/classification_report.txt", "w") as f:
    f.write(report)

print("Done! All plots saved to 'plots/' folder.")
