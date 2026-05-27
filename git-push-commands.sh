#!/bin/bash

# Commands to push to GitHub
# Run these after creating the repository on GitHub

cd /Users/loickameni/Desktop/meridian-octopus-demo

# Rename branch to main
git branch -M main

# Add remote origin
git remote add origin https://github.com/kamloicc/meridian-octopus-demo.git

# Push to GitHub
git push -u origin main

echo "✅ Repository pushed to https://github.com/kamloicc/meridian-octopus-demo"
