# CI Data

The CI data serves as a baseline for determining how a patient should perform for each exercise.

Currently, all expectations for all CI levels are based on the previous developer Michael Probst's performance of the game where the CI level 0 are Michael Probst's exact scores with a bit of a buffer to make it a bit easier.

After altering the `Assets/ScriptableObjects/InitialCIData.asset` the local data .csv files located at `C:\<your user>\AppData\LocalLow\University of Kentucky\Homebound\ci_data.csv` must be deleted. If you don’t delete this file, your changes will not be made. Note that you only need to input the CI requirements for CI level 0 for each exercise, all other CI levels have their criteria automatically generated upon creation of the `ci_data.csv` file.

The `Exercise` column is an identifier for the exercise. `Time 1`, `Successes 1` and `Misses 1` represent the baseline data for cognitive level 1. The next group, `Time 2`, `Successes 2` and `Misses 2`, represents the baseline data for cognitive level 2. The columns go on
until cognitive level 15.

