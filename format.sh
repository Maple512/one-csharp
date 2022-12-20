#!/bin/bash
git config --global core.quotepath false

dotnet format *.sln --no-restore -v diag --report ./format
