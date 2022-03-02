# == Sprint 1 Outline ==  

We're rolling with the Junkyard theme for our area.  
As of now, this is the only area planned for development for the semester.  
The focus of this sprint is to design and implement some content for an area  
with emphasis on creating memorable characters and scenery as described in the game pitch.  
We'll spend the initial half of the sprint designing stuff and the later half implementing it.  
Feel free to ask questions, as always.

---

(Ordered by Priority):  

## **Technical** (Design/Programming)
- Entity Structure Document
- Junkyard Area Draft
- More references for artists (mood boards?)

## **Enemies** (Art/Design)
- At least 5 new enemy designs
  - Can fit either the Junkyard theme, the Onyx theme, or both.
    - Onyx uses black, shades of purple, bright pink, and deep blues as main colors. 
    See [Coro and Enemy Concept Art](https://raw.githubusercontent.com/Dreamer13sq/project-kindle/main/ref/coro.png).  
      Feel free to experiment but keep total palette size as low as you can.
  - Ideas on 3/2/22: Colander Tortoise, Rusty Octopus, Trashbag Stork, Fridge Monster, Junk Mimic, Junky Jumbie (save for boss?)

## **Scenery** (Art)
- Points of interest ideas
  - Ideas on 3/2/22:
    - Magnet moving junk around
    - Giant Coro statue (made of junk or soap)
- Come up with something to interact with
  - Idea on 3/2/22: build a bridge with debris to access an area
- Design a health upgrade sprite that fits in a 32x32 tile
  - Only criteria is that it needs a heart somewhere on it

## **Gameplay** (Programming)
### Controller support
  - Bottom face button jumps
  - Left and right face buttons fire weapon
  - Top button opens a menu for items (No menu for this sprint)
  - Far shoulder button also fires weapon
  - Near shoulder buttons switch weapons
  - Both left control stick and directional pad control movement
    - Movement is boolean, meaning there is no slow walk.  
      Once the left stick passes a given threshold it is treated as a "1" value
### **Event System** 
- Pressing down to interact with a non-hostile entity triggers an event

## **New Weapon** (Design/Programming)
### Dragon Breath
- A shotgun that fires fire 
- Speed (and distance) of flames increase with level
  - Lv1 - Shoots 3 flames
  - Lv2 - Shoots 4 flames with increased distance
  - Lv3 - Shoots 5 flames with increased speed



