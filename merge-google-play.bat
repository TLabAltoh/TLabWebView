git checkout master

git status
git add .

git commit -m "tmp commit to merge into upm-google-play"

git checkout upm-google-play
git merge master --no-edit
git push origin upm

git checkout master
