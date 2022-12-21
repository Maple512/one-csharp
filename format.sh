#!/bin/bash
git config --global core.quotepath false

dotnet format ./src/*.sln --no-restore -v diag --report ./format
