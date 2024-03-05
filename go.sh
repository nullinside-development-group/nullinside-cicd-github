docker build -t nullinside-cicd-github:latest .
docker container stop nullinside-cicd-github
docker container prune -f
docker run -d --name=nullinside-cicd-github -e GITHUB_PAT=$GITHUB_PAT --restart unless-stopped nullinside-cicd-github:latest
