Using git with unity
====================


We're using github for unity with git-lfs to manage the project

git-lfs stores large assets remotely on a server and not in git history so the repo should be smaller to clone and assets don't have to be pulled as frequently. The repo should be around ~120MB with assets of ~500-600MB now. If that's too big we can remove some more assets.

Install [git-lfs](https://git-lfs.github.com/) and [github-for-unity](https://unity.github.com/)

Follow the instructions otherwise it might not work.


If you're getting rejected from the accessing the lfs server use this command


`git config --global credential.helper wincred`

You might need to turn the quality settings down in `Project Settings` -> `Quality`

Cloning a repo:
---------------
`git clone <REPO_LINK>`

To open the project first clone it, open unity, select open new project, navigate to the cloned repo and click select

Committing
-----------
Type `git status` to see what the status of all the files

Add files in git using

`git add <FILE-NAME>`

To add all files use the `-a` flag

To commit files type `git commit -m "commit message"`

Make a new branch and switch to it
-----------------

`git checkout -b <BRANCH_NAME>`

Switch to branch
-----------------

`git checkout <BRANCH_NAME>`

To merge branch Y into branch X
----------------------
Checkout into branch X

`git merge Y`

delete branch Y with

`git branch -d Y`

How to deal with merge conflicts
--------------------------------
If these happen type

`git mergetool`

To get a gui of what is clashing and use its interface to choose between what you want.

After this run

`git status`

to see all merges have been taken care of and commit your work again using

`git commit`




Workflow
========

1. When making a new feature merge from `dev` and name the branch `dev-featurename`

2. Don't mass add packages or asset folders to unity. If you need to add a standard asset or anything only add the asset you need to avoid blowing up the repo size.

3. If you need to add a large asset make sure you've tried to compress it as much as possible beforehand. Also check if the file format is included in the `.gitattributes` folder. If it isn't and its not a `.meta` file add the extension in the appropriate area.

4. Once you've finished the feature, check it works on the PC in MVB and if it does merge it with `dev`.

5. Make sure you're not committing any `dll`s or executables because they'll make the repo really big really fast.

6. Don't rename the folders outside of unity and *really* don't rename files with respect to capitalisation. This really fucks git up.

7. Use `github-for-unity`'s changes feature to review all the files you're adding to make sure you're not adding unnecessary changes.

8. Do your experimentation on your branch. Don't commit new scenes to `dev`, always stick with the base game scene. **Please** merge your branch when you're done with a feature so we don't have loads of branches that are everywhere. Delete with your branches when you're done if you don't need them anymore.
