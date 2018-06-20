#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Sturfee
{
    /// <summary>
    /// Uses Google Street View Image API to build a 6-Sided SkyBox
    /// </summary>
    public class SvToSkybox : EditorWindow
    {
        private float fov;
        private List<Direction> directions;
        private string folder;
        private SkyBoxPreference prefs;

        [MenuItem("Sturfee/Tools/StreetView To Skybox")]
        public static void Init()
        {
            SvToSkybox leWindow = GetWindowWithRect<SvToSkybox>(new Rect(0, 0, 450, 300), true, "Street View to Skybox", true);
        }

        void OnEnable()
        {
            folder = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            folder = folder.Remove(folder.Length - 20);

            fov = 90f;
            directions = new List<Direction>()
            {
                new Direction {Name = "_Front", Heading =   0, Pitch =   0},
                new Direction {Name = "_Back" , Heading = 180, Pitch =   0},
                new Direction {Name = "_Left" , Heading =  90, Pitch =   0},
                new Direction {Name = "_Right", Heading = 270, Pitch =   0},
                new Direction {Name = "_Up"   , Heading =   0, Pitch =  90},
                new Direction {Name = "_Down" , Heading =   0, Pitch = -90}
            };

            LoadPreferenceData();
        }

        void OnLostFocus()
        {
            SavePreferenceData();
        }

        void OnDestroy()
        {
            SavePreferenceData();
        }

        private static Color[] RotateMatrix(Color[] matrix, int n)
        {
            Color[] ret = new Color[n * n];

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    ret[i * n + j] = matrix[(n - j - 1) * n + i];
                }
            }

            return ret;
        }

        private static Color32[] RotateMatrix(Color32[] matrix, int n)
        {
            // rotates -90 degress
            Color32[] ret = new Color32[n * n];

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    ret[i * n + j] = matrix[(n - j - 1) * n + i];
                }
            }

            return ret;
        }

        void OnGUI()
        {
            GUILayout.Space(10);

            prefs.skyBoxName = EditorGUILayout.TextField("Skybox Name", prefs.skyBoxName);

            EditorGUILayout.Space();

            prefs.directory = EditorGUILayout.TextField("Path To Save", prefs.directory);

            EditorGUILayout.Space();

            prefs.apiKey = EditorGUILayout.TextField("API Key", prefs.apiKey);

            EditorGUILayout.Space();

            prefs.latitude = EditorGUILayout.DoubleField("Latitude", prefs.latitude);
            prefs.longitude = EditorGUILayout.DoubleField("Longitude", prefs.longitude);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Image Size to download");
            prefs.imgSize = EditorGUILayout.IntField("ImageSize", prefs.imgSize);

            GUILayout.Space(20);

            if (GUILayout.Button("Create Skybox"))
            {
                if (!Directory.Exists(prefs.directory))
                    Directory.CreateDirectory(prefs.directory);

                if (!Directory.Exists(prefs.directory + "/textures"))
                    Directory.CreateDirectory(prefs.directory + "/textures");

                Material skyboxMat = new Material(Shader.Find("Skybox/6 Sided"));

                var currentStep = 0;
                EditorUtility.DisplayProgressBar("StreetView to Skybox Progress", "Fetching images", currentStep / 6);
                foreach (Direction dir in directions)
                {
                    currentStep++;
                    EditorUtility.DisplayProgressBar("StreetView to Skybox Progress", "Fetching images", currentStep / 6);

                    GetStreetviewTexture(dir.Heading, dir.Pitch, prefs.skyBoxName + dir.Name);
                    AssetDatabase.Refresh();

                    Texture tx = AssetDatabase.LoadAssetAtPath<Texture>(Path.Combine(prefs.directory + "/textures/", prefs.skyBoxName + dir.Name + ".png"));
                    tx.wrapMode = TextureWrapMode.Clamp;

                    skyboxMat.SetTexture(dir.Name + "Tex", tx);
                }

                AssetDatabase.CreateAsset(skyboxMat, Path.Combine(prefs.directory, prefs.skyBoxName + ".mat"));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.ClearProgressBar();
            }

            GUILayout.Space(30);

            EditorGUILayout.HelpBox("Free Accounts can downloads a max image size of 640x640. Click the link below to learn more about Google's API.", MessageType.None, true);
            if (GUILayout.Button("Pricing And Plans of the Google Map API"))
            {
                Application.OpenURL("https://developers.google.com/maps/pricing-and-plans/#details");
            }
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Builds the URL, calls Google, downloads and stores the pictures
        /// </summary>
        /// <param name="heading">Heading direction for the streetview-camera</param>
        /// <param name="pitch">Pitch direction for the streetview-camera</param>
        /// <param name="imgName">Name of the image on the disk</param>
        private void GetStreetviewTexture(double heading, double pitch, string imgName)
        {
            string url = "http://maps.googleapis.com/maps/api/streetview?"
                + "size=" + prefs.imgSize + "x" + prefs.imgSize
                + "&location=" + prefs.latitude + "," + prefs.longitude
                + "&heading=" + (heading) % 360.0 + "&pitch=" + (pitch) % 360.0
                + "&fov=" + fov;

            if (prefs.apiKey != "")
                url += "&key=" + prefs.apiKey;

            WWW www = new WWW(url);

            while (!www.isDone) ;

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError("ERROR: Unable to download StreetView image: " + www.error);
            }
            else
            {
                //recived some data, store it
                byte[] bytes = www.texture.EncodeToPNG();
                File.WriteAllBytes(Path.Combine(prefs.directory + "/textures/", imgName + ".png"), bytes);

                Debug.Log(imgName + " downloaded.");
            }
        }

        private void SavePreferenceData()
        {
            try
            {
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(prefs.GetType());
                StreamWriter file = new StreamWriter(Path.Combine(folder, "preference.xml"));
                writer.Serialize(file, prefs);
                file.Close();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

        }

        private void LoadPreferenceData()
        {
            prefs = new SkyBoxPreference();
            if (File.Exists(Path.Combine(folder, "preference.xml")))
            {
                try
                {
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(prefs.GetType());
                    StreamReader file = new StreamReader(Path.Combine(folder, "preference.xml"));
                    prefs = (SkyBoxPreference)reader.Deserialize(file);
                    file.Close();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }

    public class SkyBoxPreference
    {
        //basic values if the preferences.xml was not found
        public string apiKey = "";
        public string skyBoxName = "Sturfee_Skybox";
        public string directory = "Assets/SturfeeTestTools/Skybox Tools/Static Skybox/Skyboxes";
        public double latitude = 37.3310795;
        public double longitude = -121.8882182;
        public int imgSize = 640;
    }

    class Direction
    {
        public string Name { get; set; }
        public double Heading { get; set; }
        public double Pitch { get; set; }
    }
}
#endif