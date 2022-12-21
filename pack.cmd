rmdir packages /s

dotnet build ./src/ -c Release

dotnet pack ./src/ --no-restore -c Release -o ./packages