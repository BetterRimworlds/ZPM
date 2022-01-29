#!/bin/bash

rsync -rcv --delete-after README.md ZPM/About ZPM/Defs ZPM/Patches ZPM/Textures /rimworld/1.2/Mods/ZPM/
unix2dos /rimworld/1.2/Mods/ZPM/README.md

rsync -rcv --delete-after README.md ZPM/About ZPM/Defs ZPM/Patches ZPM/Textures /rimworld/1.3/Mods/ZPM/
unix2dos /rimworld/1.3/Mods/ZPM/README.md

