# mushroomIdentifier

## przy pobraniu z eUczelnia
**mushroomIdentifier-main** zawiera w sobie cały kod

**mushroomIdentifier-model-** zawierają w sobie modele ai. należy wyodrębnić pliki ze skompresowanego folderu i umieścić je w folderze ***mushroomIdentifier-main/aiModels***

## lub pobranie bezpośredniu z repozytorium / or download directly from repository

[link to repository on github](https://github.com/KajtekWisniewski/mushroomIdentifier)

From here it is possible to download branch -main as a zip. Extract it to a folder and then proceed with the following instructions:

## Running the app

To run the whole up you have to build docker containers from the root of the project in the following process:

1. Build the AI backend:
   `docker build -t mushroom-api -f mushroom_classifier/Dockerfile .`
2. Run the AI backend contaiener:
   `docker run -d -p 5000:5000 mushroom-api`
3. **Optional** Check the status of the AI backend:
   `curl http://localhost:5000/health`
4. Run the app with docker from main folder (mushroomIdentifier):
   `docker-compose up --build`

The app will be available at localhost:5173

## Running tests

To run tests manually -> `cd mushroomAPI/`, from there run:

`dotnet test mushroomAPI.ApiTests/mushroomAPI.ApiTests.csproj`

`dotnet test mushroomAPI.UnitTests/mushroomAPI.UnitTests.csproj`

To run the tests for the AI backend run the command from the root of the project-> `python -m pytest tests/ -v`
