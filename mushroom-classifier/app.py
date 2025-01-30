from flask import Flask, request, jsonify
import tensorflow as tf
from keras.preprocessing.image import load_img, img_to_array
import numpy as np
import os
from flask_cors import CORS

app = Flask(__name__)
CORS(app)
#CORS(app, resources={r"/*": {"origins": "*", "allow_headers": "*", "methods": ["GET", "POST", "PUT", "DELETE", "OPTIONS"]}})

# local path
MODEL_PATH = "../aimodels/mushroom_classifier_model_15epochs.h5"

# docker path
# MODEL_PATH = "/app/aimodels/mushroom_classifier_model.h5"
model = tf.keras.models.load_model(MODEL_PATH)


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
class_labels = {v: k for k, v in class_indices.items()}


def preprocess_image(image_path, img_height, img_width):
    img = load_img(image_path, target_size=(img_height, img_width))
    img_array = img_to_array(img)
    img_array = np.expand_dims(img_array, axis=0)
    img_array = img_array / 255.0
    return img_array


# Predict function
def predict_mushroom_category(image_path):
    img_array = preprocess_image(image_path, 150, 150)
    predictions = model.predict(img_array)[0]

    # Get the top 5 predictions
    top_5_indices = np.argsort(predictions)[-5:][::-1]
    top_5_results = [(class_labels[i], predictions[i]) for i in top_5_indices]
    return top_5_results


@app.route("/predict", methods=["POST"])
def predict():
    if "image" not in request.files:
        return jsonify({"error": "No image file provided"}), 400

    image_file = request.files["image"]
    image_path = os.path.join("uploads", image_file.filename)
    os.makedirs("uploads", exist_ok=True)
    image_file.save(image_path)

    try:
        predictions = predict_mushroom_category(image_path)
        return (
            jsonify(
                {
                    "predictions": [
                        {"category": cat, "confidence": f"{conf * 100:.2f}%"}
                        for cat, conf in predictions
                    ]
                }
            ),
            200,
        )
    except Exception as e:
        return jsonify({"error": str(e)}), 500
    finally:
        os.remove(image_path)


if __name__ == "__main__":
    app.run(debug=True)
