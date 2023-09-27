# GenealogicalTree

A program, created for my OOP class during the second semester of my Software Engineering degree. It is an archive-like tool to manage people's family trees and information about their relatives, ancestors, and successors.

# What I Learned

- How to design a better architecture.
- How to implement custom Dependency Injection.

# How to Use It

- Download the repository and unzip it.
- Go to GenealogicalTree/bin/Debug/net6.0.
- Run GenealogicalTree.exe.
- The app has two types of accounts - admin and user. A person with a user account or with no account can only search and view people's profiles and family trees. An admin can add, remove, and edit people's profiles and family trees.
- The repository contains one user account and one admin account:
    Admin. Username - admin, password - 12345678
    User. Username - new_account, password - 12345678
- A console family tree looks like this. A person in the middle. People above them are their ancestors and people below them are their successors. This is true for every person in a tree.

    
              ├── (1)Jane Doe
              │
              ├── (0)John Doe
              │
      (2)Stew Doe
              │
              ├── (4)Lucas Doe

