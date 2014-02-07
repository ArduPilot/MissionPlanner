
1. Create a key pair

- Start 'puttygen.exe' in TortoiseGit/bin directory
 - Generate
 - Save private key (without password)
 - Copy the text for the public key
  (be sure to copy all of it)

2. Begin a fork on github

- Create a user on github.com
- Goto https://github.com/diydrones/MissionPlanner/
- Click the 'Account Settings' near the top right
 - Click 'SSH Keys' then 'Add SSH Key'
 - Write any title
 - Paste the public key text from 1
- Click the 'Fork' button near the top right to create a fork

3. Set up TortoiseGit to work with your fork

3.1 Check out your fork
- Create an empty folder anywhere
- In explorer right click and select "Git Clone"
  set URL https://github.com/<your username>/MissionPlanner
 
3.2 Fixing remote settings
- Right click the fork folder, TortoiseGit->Settings then Git->Remote
- Now edit the name origin 
   URL:       https://<your username>@github.com/<your username>/MissionPlanner.git
   Putty Key: Select your private key file from 1
   Save
- Now set a new name upstream
   Remote:    upstream
   URL:       https://github.com/diydrones/MissionPlanner
   Putty Key: empty
   Save
 
3.3 Committing changes
- Right click the fork folder, GitSync
- Do a Commit and then a Push

3.4 Updating to the most recent base
- Right click the fork folder, TortoiseGit->Pull
- Change the remote to 'upstream' 

4. Do a pull request

When everything is committed to your fork on github
 - https://github.com/<your username>/MissionPlanner/pulls
 - Click 'New pull request'
 