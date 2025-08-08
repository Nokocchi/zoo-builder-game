# Achievements
    
1: **Each achievement is part of a "line"**

This means that we can have an achievement line like "walk 1000 steps", "walk 5000 steps", "walk 10.000 steps" etc.

These could of course just be separate achievements, but since they all rely on the same data point and will be achieved in order, it makes sense to keep them grouped.

2: **Each achievement knows how close it is to being done (like, how many steps left, how many monster kills left)**

We just rely on Steam's stats. If the achievement is completed, show that. If not, fetch the stat that the achievement depends on and show that in a progress bar or something.