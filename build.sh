#!/bin/bash

# Check if inotifywait is installed
if [ -z "$(which inotifywait)" ]; then
    echo "inotifywait not installed."
    echo "Install the inotify-tools package and try again."
    exit 1
fi

# Directory to monitor for changes
dir="./Source"
MOD=$(basename $PWD)

# Define the path to your solution file
solutionPath="Source/${MOD}.sln"

# Define an array of configurations
configurations=("Release v1.2" "Release v1.3" "Release v1.4" "Release v1.5" "Release v1.6")

function build() {
    rm -rf /rimworld/1.2/Mods/${MOD}

    # Loop through each configuration and build it
    for config in "${configurations[@]}"; do
        echo "Building for configuration: $config"
        dotnet build "$solutionPath" --configuration "$config" &
    done

    wait  # Blocks until all background jobs finish

    # Copy over the mod directory.
    cp -af ${MOD} /rimworld/1.2/Mods/

    # Copy over and reformat the README.
    cp README.md /rimworld/1.2/Mods/${MOD}
    unix2dos /rimworld/1.2/Mods/${MOD}/README.md

    rm -rf /rimworld/1.3/Mods/${MOD}
    rm -rf /rimworld/1.4/Mods/${MOD}
    rm -rf /rimworld/1.5/Mods/${MOD}
    rm -rf /rimworld/1.6-steam/Mods/${MOD}

    cp -af /rimworld/1.2/Mods/${MOD} /rimworld/1.3/Mods
    cp -af /rimworld/1.2/Mods/${MOD} /rimworld/1.4/Mods
    cp -af /rimworld/1.2/Mods/${MOD} /rimworld/1.5/Mods
    cp -af /rimworld/1.2/Mods/${MOD} /rimworld/1.6-steam/Mods

    echo "All builds completed!"
}

build

if [ "$1" == "1" ]; then
    echo "Done"
    exit
fi

# Watch for changes to .cs files in the directory and subdirectories
inotifywait --recursive --monitor --format "%e %w%f" \
    --event modify,move,create,delete $dir \
    --include '\.cs$' |
    while read changed; do
        echo "Detected change in $changed"
        build
    done

