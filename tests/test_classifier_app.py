import io
import pytest
from mushroom_classifier.app import app


@pytest.fixture
def client():
    with app.test_client() as client:
        yield client


class TestApp:
    def test_no_file_uploaded(self, client):
        response = client.post("/predict")
        assert response.status_code == 400
        assert "error" in response.get_json()

    def test_valid_image_prediction(self, client, mocker):
        mocker.patch(
            "mushroom_classifier.app.predict_mushroom_category",
            return_value=[("Amanita", 0.85), ("Boletus", 0.1)],
        )

        with open("testImages/test.jpg", "rb") as img_file:
            data = {"image": (io.BytesIO(img_file.read()), "sample.jpg")}
            response = client.post(
                "/predict", data=data, content_type="multipart/form-data"
            )

        assert response.status_code == 200
        json_data = response.get_json()
        assert "predictions" in json_data
        assert len(json_data["predictions"]) > 0
        assert json_data["predictions"][0]["category"] == "Amanita"

    def test_health_endpoint(self, client):
        response = client.get("/health")
        assert response.status_code == 200
        json_data = response.get_json()
        assert json_data == {"status": "ok"}
