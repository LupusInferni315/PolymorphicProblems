using UnityEngine;

public enum NameGender
{
    Masculine,
    Feminine,
    Neutral
}

public interface INameGenerator
{
    string GenerateName ( NameGender gender );
}