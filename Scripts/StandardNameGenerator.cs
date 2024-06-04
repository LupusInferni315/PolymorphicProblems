using System.ComponentModel;
using System.Linq;
using UnityEngine;

[System.Serializable, DefaultPolymorphicType]
public class StandardNameGenerator : INameGenerator
{
    #region Declarations

    [System.Serializable]
    public class NamePart
    {
        [SerializeField] private string [] m_GivenNames = new string [ 0 ];
        [SerializeField] private string [] m_MiddleNames = new string [ 0 ];

        public string [] GivenNames { get => m_GivenNames; set => m_GivenNames = value; }
        public string [] MiddleNames { get => m_MiddleNames; set => m_MiddleNames = value; }
    }

    #endregion

    #region Fields

    [SerializeField] private NamePart m_Feminine = new ();
    [SerializeField] private NamePart m_Masculine = new ();
    [SerializeField] private NamePart m_Neutral = new ();

    [SerializeField] private string [] m_FamilyNames = new string [ 0 ];

    #endregion

    #region Methods

    private string [] GenerateGivenNames ( NameGender gender )
    {
        return gender switch
        {
            NameGender.Masculine => Enumerable.Concat ( m_Masculine.GivenNames, m_Neutral.GivenNames ).ToArray (),
            NameGender.Feminine => Enumerable.Concat ( m_Feminine.GivenNames, m_Neutral.GivenNames ).ToArray (),
            NameGender.Neutral => m_Neutral.GivenNames,
            _ => throw new InvalidEnumArgumentException ( nameof ( gender ), (int) gender, typeof ( NameGender ) ),
        };
    }

    private string [] GenerateMiddleNames ( NameGender gender )
    {
        return gender switch
        {
            NameGender.Masculine => Enumerable.Concat ( m_Masculine.MiddleNames, m_Neutral.MiddleNames ).ToArray (),
            NameGender.Feminine => Enumerable.Concat ( m_Feminine.MiddleNames, m_Neutral.MiddleNames ).ToArray (),
            NameGender.Neutral => m_Neutral.MiddleNames,
            _ => throw new InvalidEnumArgumentException ( nameof ( gender ), (int) gender, typeof ( NameGender ) ),
        };
    }

    public string GenerateName ( NameGender gender )
    {
        string [] givenNames = GenerateGivenNames ( gender );
        string [] middleNames = GenerateMiddleNames ( gender );
        string [] familyNames = m_FamilyNames;

        return $"{givenNames [ Random.Range ( 0, givenNames.Length ) ]} {middleNames [ Random.Range ( 0, middleNames.Length ) ]} {familyNames [ Random.Range ( 0, familyNames.Length ) ]}";
    }

    #endregion
}