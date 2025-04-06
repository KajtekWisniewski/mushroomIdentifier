import kagglehub
import os
import tensorflow as tf
from keras.models import Sequential
from keras.layers import Conv2D, MaxPooling2D, Flatten, Dense, Dropout
from tensorflow.keras.preprocessing.image import ImageDataGenerator
from PIL import ImageFile


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
data_gen = ImageDataGenerator(rescale=1.0 / 255, validation_split=0.2)

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
        Flatten(),
        Dense(128, activation="relu"),
        Dropout(0.5),
        Dense(len(train_data.class_indices), activation="softmax"),
    ]
)

# Compile the model
model.compile(optimizer="adam", loss="categorical_crossentropy", metrics=["accuracy"])

# Train the model
print("Training model...")
history = model.fit(train_data, validation_data=validation_data, epochs=20)

# Save the model
model_save_path = "aiModels/mushroom_classifier_model_15epochs.h5"
model.save(model_save_path)
print(f"Model saved to {model_save_path}")

# Evaluate the model
print("Evaluating model...")
evaluation = model.evaluate(validation_data)
print(f"Validation Loss: {evaluation[0]}, Validation Accuracy: {evaluation[1]}")
