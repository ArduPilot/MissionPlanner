set GIT_TRACE=2
set GIT_SSH_COMMAND=ssh -v
git tag -d beta

git push origin :refs/tags/beta

git tag beta

git push origin beta

pause
