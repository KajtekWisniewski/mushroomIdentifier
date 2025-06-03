import kagglehub
import os
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.neighbors import KNeighborsClassifier
from sklearn.metrics import accuracy_score
from tensorflow.keras.preprocessing.image import load_img, img_to_array
from PIL import ImageFile

print("Downloading dataset from Kaggle...")
data_dir = kagglehub.dataset_download(
    "maysee/mushrooms-classification-common-genuss-images"
)

mushrooms_dir = os.path.join(data_dir, "Mushrooms")
print(data_dir)

# Define image parameters
img_height, img_width = 150, 150

ImageFile.LOAD_TRUNCATED_IMAGES = True

# Prepare dataset
print("Preparing dataset...")

X = []
y = []
class_names = sorted(os.listdir(mushrooms_dir))
label_map = {class_name: i for i, class_name in enumerate(class_names)}

for class_name in class_names:
    class_dir = os.path.join(mushrooms_dir, class_name)
    for fname in os.listdir(class_dir):
        img_path = os.path.join(class_dir, fname)
        img = load_img(img_path, target_size=(img_height, img_width))
        img_array = img_to_array(img).flatten() / 255.0  # Normalize
        X.append(img_array)
        y.append(label_map[class_name])

X = np.array(X)
y = np.array(y)

# Create train-validation split
X_train, X_val, y_train, y_val = train_test_split(X, y, test_size=0.2, random_state=42)

# Build k-NN model
print("Building k-NN model...")
knn_clf = KNeighborsClassifier(n_neighbors=5)

# Train the model
print("Training model...")
knn_clf.fit(X_train, y_train)

# Evaluate the model
print("Evaluating model...")
y_pred = knn_clf.predict(X_val)
accuracy = accuracy_score(y_val, y_pred)
print(f"k-NN Validation Accuracy: {accuracy:.2f}")
