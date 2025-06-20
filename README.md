# mushroomIdentifier

## Running the app

To run the whole up you have build docker containers in the following process:

1. Build the AI backend:
 `docker build -t mushroom-api mushroom_classifier/.`
2. Run the AI backend contaiener:
 `docker run -d -p 5000:5000 mushroom-api`
3. **Optional** Check the status of the AI backend:
 `curl http://localhost:5000/health` 
4. Run the app with docker from main folder (mushroomIdentifier):
 `docker-compose up --build`

## Running tests

To run tests manually -> `cd mushroomAPI/`, from there run:

`dotnet test mushroomAPI.ApiTests/mushroomAPI.ApiTests.csproj`

`dotnet test mushroomAPI.UnitTests/mushroomAPI.UnitTests.csproj`

To run the tests for the AI backend -> `cd tests`
