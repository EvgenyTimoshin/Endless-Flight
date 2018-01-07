namespace DCM.Old {
    [System.Obsolete("This Class is obsolete")]
    [System.Serializable]
    public class ShaderProperties {
        public bool markAsNormal;
        public string propertyName;
    
        public ShaderProperties(bool normal, string name) {
            markAsNormal = normal;
            propertyName = name;
        }
    }
}
