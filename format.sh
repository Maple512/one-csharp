#!/bin/bash
git config --global core.quotepath false

dotnet format --no-restore -v diag --report ./format
