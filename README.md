# Download Glacial Images
Downloads the images from the server one at a time.

## Dependencies
- https://github.com/jonkeegan/nagap-aerial-glacier-photographs
- The above github repo is necessary especially the nagap_glacier_photos.csv file.

## Instructions
- Clone this repo
- install .net runtime or Visual Studio (https://visualstudio.microsoft.com/vs/community/)
- Put the csv file in a known location
- open the code and modify the filename and outpath for your system
- Open sln and build or run 'dotnet build downloadimages.sln'
- run and enjoy the fotos
- My fav to date: AP69v2_198 (those shadows!)

## Restartable
- Also as long as you have not moved any files you can restart the app and it will skip to the current download
- It won't re-download any images you've already downloaded
