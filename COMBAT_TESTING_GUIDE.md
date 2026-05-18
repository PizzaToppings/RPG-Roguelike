# Combat Testing System

This document explains how to use the Combat Testing System to quickly test combat scenarios without going through the full game loop.

## Overview

The Combat Testing System allows you to:
- Start the game directly in the Combat scene
- Configure preset or randomized encounters
- Configure preset or randomized party compositions
- Configure preset or randomized skills for each character
- Configure preset or randomized trinkets for each character
- Set starting conditions (HP, gold, etc.)

## Components

### 1. SO_CombatTestConfig (ScriptableObject)
This is the configuration file that defines what should be loaded when testing combat.

**Location:** Create via `Right-click in Project > Create > ScriptableObjects > Testing > CombatTestConfig`

### 2. CombatTestManager (MonoBehaviour)
This script reads the test configuration and populates RunData before combat starts.

**Location:** `Assets/Scripts/Testing/CombatTestManager.cs`

## Setup Instructions

### Step 1: Create a Test Configuration

1. In the Project window, navigate to `Assets/Scriptable Objects/`
2. Right-click and select `Create > ScriptableObjects > Testing > CombatTestConfig`
3. Name it something descriptive (e.g., "TestConfig_BasicCombat")

### Step 2: Configure the Test Settings

Open your test config asset and configure the following:

#### Test Mode
- **Enable Test Mode**: Check this to activate testing mode

#### Encounter Configuration
- **Preset Encounter**: Drag an SO_Encounter asset here to use a specific encounter
- **Randomize Encounter**: Check to randomly select from the encounter pool
- **Encounter Pool**: Drag an SO_EncounterPool asset here (if randomizing)

#### Character Configuration
- **Preset Characters**: Drag SO_Character assets here to use specific characters
- **Randomize Characters**: Check to randomly select from the character roster
- **Min/Max Character Count**: Set the range for random party size (1-4)
- **Character Roster**: Drag an SO_CharacterRoster asset here (if randomizing)

#### Skills Configuration
- **Randomize Skills**: Check to randomly assign skills to characters
- **Min/Max Skills Per Character**: Set the range for random skill count (0-4)
- **Skill Pool**: Drag an SO_SkillPool asset here (if randomizing)
- **Preset Skills**: Manually configure skills per character (if not randomizing)
  - Add entries to the list (one per character)
  - Each entry can have up to 4 skills

#### Trinkets Configuration
- **Randomize Trinkets**: Check to randomly assign trinkets to characters
- **Min/Max Trinkets Per Character**: Set the range for random trinket count (0-4)
- **Trinket Pool**: Drag an SO_TrinketPool asset here (if randomizing)
- **Preset Trinkets**: Manually configure trinkets per character (if not randomizing)
  - Add entries to the list (one per character)

#### Starting Conditions
- **Starting HP Percentage**: Set starting HP for all characters (0-100, 0 = full HP)
- **Starting Gold**: Set starting gold amount

### Step 3: Add CombatTestManager to Combat Scene

1. Open your Combat scene
2. Create a new empty GameObject (Right-click in Hierarchy > Create Empty)
3. Name it "CombatTestManager"
4. Add the `CombatTestManager` component to it
5. Drag your test config asset into the "Test Config" field

### Step 4: Verify Script Execution Order

The CombatTestManager must run before EncounterManager and PartyManager.

1. Go to `Edit > Project Settings > Script Execution Order`
2. Verify that `CombatTestManager` has an execution order of **-100**
3. If not, add it manually:
   - Click the `+` button
   - Select `CombatTestManager`
   - Set execution order to `-100`
   - Click Apply

## Usage Examples

### Example 1: Test a Specific Encounter with Random Party

```
✓ Enable Test Mode
Preset Encounter: [Elite_Encounter_01]
✗ Randomize Encounter

✗ Preset Characters (leave empty)
✓ Randomize Characters
Min Character Count: 2
Max Character Count: 3
Character Roster: [MainCharacterRoster]

✓ Randomize Skills
Min Skills Per Character: 2
Max Skills Per Character: 4
Skill Pool: [MainSkillPool]

✓ Randomize Trinkets
Min Trinkets Per Character: 0
Max Trinkets Per Character: 2
Trinket Pool: [MainTrinketPool]

Starting HP Percentage: 100
Starting Gold: 0
```

### Example 2: Test Specific Characters with Preset Skills

```
✓ Enable Test Mode
Preset Encounter: [Boss_Encounter_01]
✗ Randomize Encounter

Preset Characters:
  - [0] Character_Warrior
  - [1] Character_Mage
  - [2] Character_Rogue
✗ Randomize Characters

✗ Randomize Skills
Preset Skills:
  - [0] Skills: [Skill_Shield_Bash, Skill_Whirlwind, Skill_Rally]
  - [1] Skills: [Skill_Fireball, Skill_Ice_Lance, Skill_Teleport]
  - [2] Skills: [Skill_Backstab, Skill_Vanish, Skill_Poison_Dagger]

✗ Randomize Trinkets
Preset Trinkets:
  - [0] Trinkets: [Trinket_Warriors_Ring]
  - [1] Trinkets: [Trinket_Spell_Focus]
  - [2] Trinkets: [Trinket_Shadow_Cloak]

Starting HP Percentage: 50
Starting Gold: 100
```

### Example 3: Fully Random Test

```
✓ Enable Test Mode
✗ Preset Encounter (leave empty)
✓ Randomize Encounter
Encounter Pool: [NormalEncounterPool]

✗ Preset Characters (leave empty)
✓ Randomize Characters
Min Character Count: 1
Max Character Count: 4
Character Roster: [MainCharacterRoster]

✓ Randomize Skills
Min Skills Per Character: 1
Max Skills Per Character: 4
Skill Pool: [MainSkillPool]

✓ Randomize Trinkets
Min Trinkets Per Character: 1
Max Trinkets Per Character: 3
Trinket Pool: [MainTrinketPool]

Starting HP Percentage: 100
Starting Gold: 50
```

## Testing Workflow

### Quick Testing in Combat Scene

1. Open the Combat scene in Unity
2. Make sure CombatTestManager is in the scene with a valid config
3. Press Play
4. The system will automatically populate RunData with your test configuration
5. Combat will start with your specified setup

### Switching Between Test and Normal Mode

**To Enable Test Mode:**
- Check "Enable Test Mode" in your test config

**To Disable Test Mode:**
- Uncheck "Enable Test Mode" in your test config
- OR remove the test config from CombatTestManager
- OR remove the CombatTestManager GameObject from the scene

**Normal Game Flow:**
When test mode is disabled or when RunData is already populated (normal game flow), the CombatTestManager does nothing and lets the normal systems work.

## Troubleshooting

### Test mode isn't activating

**Check:**
1. Is "Enable Test Mode" checked in your test config?
2. Is the test config assigned to CombatTestManager?
3. Is CombatTestManager in the Combat scene?
4. Is the script execution order set correctly (-100)?

### Characters/Enemies not spawning

**Check:**
1. Are your preset characters/encounters assigned correctly?
2. If randomizing, are the pools (roster, skill pool, etc.) assigned and not empty?
3. Check the Console for warning messages from CombatTestManager

### Skills/Trinkets not working

**Check:**
1. Are the skills/trinkets assigned correctly in the test config?
2. If randomizing, are the min/max values set correctly?
3. Check if class restrictions are preventing assignment (console logs will show this)

### Wrong encounter is loading

**Check:**
1. If you want a specific encounter, make sure "Preset Encounter" is assigned
2. If you want random, make sure "Randomize Encounter" is checked AND "Preset Encounter" is empty
3. Make sure the EncounterManager in the scene is configured correctly

## Advanced Tips

### Multiple Test Configs

Create multiple test configuration assets for different scenarios:
- `TestConfig_EarlyGame` - 1-2 characters, basic skills
- `TestConfig_MidGame` - 3-4 characters, mixed skills and trinkets
- `TestConfig_BossFight` - Full party, best gear, boss encounter
- `TestConfig_StressTest` - Random everything for variety testing

Switch between them by changing which config is assigned to CombatTestManager.

### Testing Specific Mechanics

**Testing Skills:**
- Create a config with 1 character and only the skills you want to test
- Use a simple encounter with weak enemies

**Testing Trinkets:**
- Create a config with specific trinket combinations
- Set starting HP to 50% to test healing trinkets
- Use different encounter tiers to test combat trinkets

**Testing Character Builds:**
- Create configs for different class combinations
- Test synergies between skills and trinkets
- Experiment with different party sizes

### Console Logging

The system outputs detailed logs to the Console when initializing:
```
[CombatTest] Initializing combat test mode...
[CombatTest] Using preset encounter: Elite_Encounter_01
[CombatTest] Using 3 preset character(s).
[CombatTest] Character 0 (Warrior): Randomized 4 skill(s).
[CombatTest] Character 1 (Mage): Using 3 preset skill(s).
[CombatTest] Test initialization complete. Party size: 3, Encounter: Elite_Encounter_01
```

Use these logs to verify your configuration is working as expected.

## Notes

- The test system only activates when starting directly in the Combat scene with empty RunData
- During normal gameplay (going through menus), the test system is completely bypassed
- Test configs can be version controlled and shared with the team
- The system respects class restrictions when randomizing skills and trinkets
- Preset configurations take precedence over randomization settings
