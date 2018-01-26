# ArkWatch
A tool to monitor online players and tribes on ARK: Survival Evolved server.

**Project is currently WORK IN PROGRESS with only minimal features.**
## Features
* Show online players
* (WIP) Show online tribes and their player count
* (WIP) Collect statistcs of online time for each player and tribe over time (to determine playing hours)
* (WIP) Enable alerts for specific tribes if a lot of their members are online
## Implementation
### Querying player data
Querying of player data is done using an implementation of Steam Server Query protocol.
### Data storage
Data such as saved servers and players assigned to tribes is seralized in JSON format and stored localy (currently in `strorage.json` file).
