﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RobotNames {

	private static RobotNames instance = null;
	private  Dictionary<string, Name> robotNames = new Dictionary<string, Name>();
	private int survivorNamesUsed = 0;

	private RobotNames() {
		ResetNames ();
	}

	public static RobotNames Instance {
		get { 
			if (instance == null) 
				instance = new RobotNames ();
			return instance;
		}
	}

	public int numRobotNames {
		get { 
			return robotNames.Count;
		}
	}

	public void ResetNames() {
		survivorNamesUsed = 0;
		robotNames.Clear ();
		foreach (string name in rawNames) {
			robotNames.Add (name, new Name (name));
		}
	}

	public void ResetSurvivorNamesUsed() {
		survivorNamesUsed = 0;
	}

	public void AddRobotSurvivalTime(string name, float timeSurvived, bool died = false, MethodOfDeath howRobotDied = MethodOfDeath.SURVIVED) {
		Name robotName = robotNames [name];
		robotNames [name] = new Name (name, true, died, robotName.timeSurvived + timeSurvived, robotName.boxesDelivered, howRobotDied);
	}

	public void AddRobotBoxDelivery(string name) {
		Name robotName = robotNames [name];
		robotNames [name] = new Name (name, true, false, robotName.timeSurvived, robotName.boxesDelivered + 1);
	}

	public List<string> GetObituaries() {
		List<string> obituaries = new List<string> ();

		// sort by boxesDelivered then timeSurvived
		List<Name> sortedNames = new List<Name> ();
		foreach (KeyValuePair<string, Name> pair in robotNames) {
			if (pair.Value.died)
				sortedNames.Add (pair.Value);
		}
		NameComparer nc = new NameComparer ();
		sortedNames.Sort (nc);

		foreach (Name robot in sortedNames) {
			obituaries.Add (robot.name + " delivered " + robot.boxesDelivered + " boxes, " + GetDeathString(robot.howDied) + Mathf.RoundToInt(robot.timeSurvived) + " seconds.");
		}
		return obituaries;
	}

	private string GetDeathString(MethodOfDeath howRobotDied) {
		switch (howRobotDied) {
			case MethodOfDeath.DEATH_BY_CRUSHER:
				return "and was crushed after ";
			case MethodOfDeath.DEATH_BY_FIRE:
				return "and got fired after ";
			case MethodOfDeath.DEATH_BY_PIT:
				return "and fell in a pit after ";
			case MethodOfDeath.DEATH_BY_BOMB:
				return "and was obliterated after ";
			default:
				return "and continued to live? after ";
		}
	}

	public string TryGetSurvivorName () {
		int skipNameCount = 0;
		foreach (KeyValuePair<string, Name> pair in robotNames) {
			if (pair.Value.used && !pair.Value.died && survivorNamesUsed <= skipNameCount++) {
				survivorNamesUsed++;
				return pair.Value.name;
			}
		}
		survivorNamesUsed++;
		HUDManager.instance.BuildRobot ();
		return GetUnusedName ();
	}

	private string GetUnusedName() {
		string tryName = null;
		int tryCount = 0;
		bool used = false;
		do {
			tryName = rawNames [Random.Range (0, rawNames.Length)];
			used = robotNames[tryName].used;
			tryCount++;
		} while (used && tryCount < robotNames.Count);

		if (!used) {
			robotNames [tryName] = new Name (tryName, true);
			return tryName;
		} else {	// the list is running low, stop trying randomly, just find an unused one
			foreach (KeyValuePair<string, Name> pair in robotNames) {
				if (!pair.Value.used) {
					robotNames[pair.Value.name] = new Name (pair.Value.name, true);
					return pair.Value.name;
				}
			}
		}
		return "Ghost";		// FIXME(~): GameOver should occur before this happens (Ghost has no Name properties for the obituaries and will cause game breaking exceptions)
	}

	public int maxAvailableNames {
		get {
			return rawNames.Length;
		}
	}

	public struct Name {
		public string name;
		public bool used;
		public bool died;
		public float timeSurvived;
		public int boxesDelivered;
		public MethodOfDeath howDied;

		public Name(string _name, bool _used = false, bool _died = false, float _timeSurvived = 0.0f, int _boxesDelivered = 0, MethodOfDeath _howDied = MethodOfDeath.SURVIVED) {
			name = _name;
			used = _used;
			died = _died;
			timeSurvived = _timeSurvived;
			boxesDelivered = _boxesDelivered;
			howDied = _howDied;
		} 
	};

	public enum MethodOfDeath {
		SURVIVED,
		DEATH_BY_CRUSHER,
		DEATH_BY_PIT,
		DEATH_BY_FIRE,
		DEATH_BY_BOMB
	};

	// 60 names
	private string[] rawNames = {
		"Bob",
		"Ethan",
		"Ruptert",
		"Nathan",
		"Olivia",
		"John",
		"Jade",
		"Quincy",
		"Dilbert",
		"Sarah",
		"Kristen",
		"Blert",
		"Nine",
		"Fish",
		"Kate",
		"Jessica",
		"Doris",
		"Betty",
		"Jack",
		"K9",
		"Ronald",
		"Jorsh",
		"Tom",
		"Brandon",
		"Russell",
		"Atron",
		"Kevin",
		"Kyle",
		"Jarett",
		"Nikolai",
		"Sebastian",
		"Ana",
		"Devin",
		"Bread",
		"Rerun",
		"Radar",
		"Domo",
		"Roboto",
		"Lucy",
		"Gladis",
		"Mlem",
		"Rick",
		"Maureen",
		"Mike",
		"Kit",
		"Kat",
		"Nora",
		"Keaton",
		"Kathy",
		"Mosh",
		"Dobble",
		"Diskette",
		"Disk",
		"Morty",
		"Dice",
		"Robert Paulson",
		"Armondo",
		"Emily",
		"Zeek",
		"Allons-y Alonso"
	};

	public class NameComparer: Comparer<Name> {
		public override int Compare (Name a, Name b) {
			if (a.boxesDelivered > b.boxesDelivered || (a.boxesDelivered == b.boxesDelivered && a.timeSurvived > b.timeSurvived))
				return -1;
			else if (a.boxesDelivered < b.boxesDelivered || (a.boxesDelivered == b.boxesDelivered && a.timeSurvived < b.timeSurvived))
				return 1;
			return 0;
		}
	}
}