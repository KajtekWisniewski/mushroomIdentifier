import tensorflow as tf
from keras.preprocessing.image import load_img, img_to_array
import numpy as np


# Function to preprocess input image
def preprocess_image(image_path, img_height, img_width):
    img = load_img(image_path, target_size=(img_height, img_width))  # Resize the image
    img_array = img_to_array(img)  # Convert to array
    img_array = np.expand_dims(img_array, axis=0)  # Add batch dimension
    img_array = img_array / 255.0  # Normalize pixel values
    return img_array


# Predict function
def predict_mushroom_category(image_path, model_path, class_indices):
    # Load the trained model
    model = tf.keras.models.load_model(model_path)

    # Preprocess the input image
    img_array = preprocess_image(image_path, 224, 244)

    # Make prediction
    predictions = model.predict(img_array)[0]
    top_5_indices = np.argsort(predictions)[-5:][::-1]
    class_labels = {v: k for k, v in class_indices.items()}

    top_5_results = [(class_labels[i], predictions[i]) for i in top_5_indices]
    return top_5_results


# Example usage (update paths as needed)
if __name__ == "__main__":
    image_path = "testImages/test.jpg"
    model_path = "aiModels/mushroom_classifier_model_15epochs.h5"

    class_indices = {
        "Agaricus": 0,
        "Amanita": 1,
        "Boletus": 2,
        "Cortinarius": 3,
        "Entoloma": 4,
        "Hygrocybe": 5,
        "Lactarius": 6,
        "Russula": 7,
        "Suillus": 8,
    }

    # Predict the category
    top_5_results = predict_mushroom_category(image_path, model_path, class_indices)

    print("Top 5 predictions:")
    for rank, (category, confidence) in enumerate(top_5_results, start=1):
        print(f"{rank}. {category} - {confidence * 100:.2f}%")
