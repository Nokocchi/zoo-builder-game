# Ideal requirements
    
1: **Each achievement must be part of a "line"**

This means that we can have an achievement line like "walk 1000 steps", "walk 5000 steps", "walk 10.000 steps" etc.

These could of course just be separate achievements, but since they all rely on the same data point and will be achieved in order, it makes sense to keep them grouped.

2: **Each achievement must be able to know how close it is to being done (like, how many steps left, how many monster kills left)**

Couple options:

- 1: I can make an interface that defines a function that checks progress (maybe checks GameData singleton, etc.)
- 2: I can store the data that the achievement relies on in the parent (achievement line) instance

Not a big fan of 2. Maybe I would like the axe upgrades and power to depend on the number of trees cut, and it feels weird to have to check a specific achievement line for the number of trees cut..

3: **When an achievement has been achieved, do an in-game popup and save it so the popup only happens once**

I will probably use the SteamWorks API to do the popups..

4: **Assuming I will use SteamWorks API, maybe I don't need to implement anything myself**

- I could probably handle the achievement line logic through some ugly naming schemes. Either way, I plan on using Steam's APIs
so I might as well try to find a solution for the achievements that works with the Steam API as well.