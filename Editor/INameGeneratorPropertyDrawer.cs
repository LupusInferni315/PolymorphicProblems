using UnityEditor;

[CustomPropertyDrawer ( typeof ( INameGenerator ), true )]
public class INameGeneratorPropertyDrawer : PolymorphicPropertyDrawer<INameGenerator>
{
    protected override string PostProcessName ( string name )
    {
        return name.Replace ( "NameGenerator", "" );
    }
}