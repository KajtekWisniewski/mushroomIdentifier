FROM python:3.11-slim

RUN apt-get update && apt-get install -y \
    libgl1-mesa-glx \
    libglib2.0-0 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY mushroom_classifier/requirements.txt .

RUN pip install --no-cache-dir -r requirements.txt

COPY mushroom_classifier/ .

COPY ../aiModels/mushroom_classifier_model_30epochs_final_paper.h5 /app/

RUN mkdir -p uploads

EXPOSE 5000

CMD ["python", "app.py"]