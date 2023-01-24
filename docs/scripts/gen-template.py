import os, sys

for root, dirs, files in os.walk(sys.argv[1]):
    root_split = os.path.split(root)
    if root_split[-1].startswith("Merseyrail"):
        print("\n## " + root_split[-1])
    for file in files:
        if file.endswith(".cs"):
            print("### " + file)
