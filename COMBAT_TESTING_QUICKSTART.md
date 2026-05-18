# Combat Testing - Quick Start Guide

## 1. Create Test Config
Right-click in Project → `Create > ScriptableObjects > Testing > CombatTestConfig`

## 2. Configure Settings
- ✓ **Enable Test Mode**
- Set **Preset Encounter** OR check **Randomize Encounter**
- Set **Preset Characters** OR check **Randomize Characters** with min/max counts
- Check **Randomize Skills** with min/max per character (or use preset)
- Check **Randomize Trinkets** with min/max per character (or use preset)
- Set **Starting HP Percentage** and **Starting Gold**

## 3. Add to Combat Scene
1. Create empty GameObject named "CombatTestManager"
2. Add `CombatTestManager` component
3. Drag your test config into the "Test Config" field

## 4. Verify Execution Order
`Edit > Project Settings > Script Execution Order`
- Ensure `CombatTestManager` is set to **-100**

## 5. Play!
Press Play in Combat scene. Check Console for initialization logs.

## Quick Config Examples

### Random Everything
```
✓ Enable Test Mode
✓ Randomize Encounter (Encounter Pool assigned)
✓ Randomize Characters (1-4 characters)
✓ Randomize Skills (2-4 per character)
✓ Randomize Trinkets (0-2 per character)
Starting HP: 100%
```

### Specific Test Setup
```
✓ Enable Test Mode
Preset Encounter: [Your_Encounter]
Preset Characters: [Char1, Char2, Char3]
Preset Skills: [Configured per character]
Preset Trinkets: [Configured per character]
Starting HP: 50%
```

For detailed documentation, see [COMBAT_TESTING_GUIDE.md](COMBAT_TESTING_GUIDE.md)
