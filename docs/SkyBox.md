# Sky Box

I've used the [Stylized Sky shader](https://github.com/gdquest-demos/godot-4-stylized-sky) by
GDQuest Demos.

This shader supports many different things in a lot of details, but at the moment I just have a simple setup
for the clouds, sky color, light source image, stars, etc.

The day-night cycle is basically just one big animation with morning, day, afternoon and night keys. 
There is only one light source. When the sun reaches the horizon, it immediately teleports back to where it started, 
and its texture is replaced with a moon image. The same thing happens when the moon reaches the horizon and morning comes.