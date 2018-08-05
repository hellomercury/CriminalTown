using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.ObjectModel;

namespace CriminalTown {

    [Serializable]
    public class SData {
        public int[] ItemsCount;
        public int Money;
    }

    [Serializable]
    public class ChData {
        private readonly List<Character> m_campCharacters;
        private readonly List<Character> m_panelCharacters;

        public ReadOnlyCollection<Character> PanelCharacters {
            get {
                return m_panelCharacters.AsReadOnly();
            }
        }

        public List<Character> CampCharacters {
            get {
                return m_campCharacters;
            }
        }

        public delegate void ChDataEvent(Character character);

        public event ChDataEvent OnRemoveEvent = delegate { };

        public event ChDataEvent OnAddEvent = delegate { };

        //Constructor
        public ChData() {
            m_campCharacters = new List<Character>();
            m_panelCharacters = new List<Character>();
        }

        public void RemoveCharacter(Character character) {
            m_panelCharacters.Remove(character);
            OnRemoveEvent(character);
        }

        public void AddCharacter(Character character) {
            m_panelCharacters.Add(character);
            OnAddEvent(character);
        }

    }

    [Serializable]
    public class PData {
        public bool[] IsItemAvailable;
        public bool IsBlackMarketAvailable;
        public bool IsBanditCampAvailable;
        public bool IsHospitalAvailable;
        public bool IsPoliceStationAvailable;
        public bool IsRobberyAvailable; //В дальнейшем добавить виды ограблений

        public int Authority;
        public int Day;
    }

    [Serializable]
    public class EData {
        public int PoliceKnowledge;

        public Dictionary<RobberyType, Dictionary<int, Robbery>> RobberiesData;
    }

    public static class DataScript {
        public static SData SData = new SData();
        public static ChData ChData = new ChData();
        public static PData PData = new PData();
        public static EData EData = new EData();

        public static bool CheckDataFiles() {
            return
                (File.Exists(Application.persistentDataPath + "/sourcesDataFile.dat") &&
                    File.Exists(Application.persistentDataPath + "/charactersDataFile.dat") &&
                    File.Exists(Application.persistentDataPath + "/progressDataFile.dat")) &&
                File.Exists(Application.persistentDataPath + "/eventsDataFile.dat");
        }

        public static void LoadData() {
            SData = (SData) LoadData("/sourcesDataFile.dat");
            ChData = (ChData) LoadData("/charactersDataFile.dat");
            PData = (PData) LoadData("/progressDataFile.dat");
            EData = (EData) LoadData("/eventsDataFile.dat");
        }

        public static void SaveSourcesData() {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream dataFile = File.Create(Application.persistentDataPath + "/sourcesDataFile.dat");
            bf.Serialize(dataFile, SData);
            dataFile.Close();
        }

        public static void SaveCharactersData() {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream dataFile = File.Create(Application.persistentDataPath + "/charactersDataFile.dat");
            bf.Serialize(dataFile, ChData);
            dataFile.Close();
        }

        public static void SaveProgressData() {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream dataFile = File.Create(Application.persistentDataPath + "/progressDataFile.dat");
            bf.Serialize(dataFile, PData);
            dataFile.Close();
        }

        public static void SaveEventsData() {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream dataFile = File.Create(Application.persistentDataPath + "/eventsDataFile.dat");
            bf.Serialize(dataFile, EData);
            dataFile.Close();
        }

        public static void SaveAll() {
            SaveCharactersData();
            SaveEventsData();
            SaveProgressData();
            SaveSourcesData();
        }

        private static object LoadData(string fileName) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream dataFile = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
            object data = bf.Deserialize(dataFile);
            dataFile.Close();
            return data;
        }
    }

}