#!/bin/bash

rsync -rcv --delete-after README.md ZPM/About ZPM/Defs ZPM/Patches ZPM/Textures /c/RimWorld1-2-2900Win64/Mods/ZPM/
unix2dos /c/RimWorld1-2-2900Win64/Mods/ZPM/README.md

# Remove all of the extra assemblies.
find /c/RimWorld1-2-2900Win64/Mods/ZPM/Assemblies ! -name 'ZPM.dll' -type f -exec rm -vf {} +


rsync -rcv --delete-after /c/RimWorld1-2-2900Win64/Mods/ZPM "/c/Program Files (x86)/Steam/steamapps/common/RimWorld/Mods/"
