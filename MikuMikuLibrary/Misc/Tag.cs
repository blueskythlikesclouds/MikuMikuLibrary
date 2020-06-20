using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace MikuMikuLibrary.Misc
{
    public class Tag
    {
        public string Key { get; set; }
        public List<string> Values { get; }

        public static Tag Parse( string s )
        {
            if ( !s.StartsWith( "@" ) )
                return null;

            var tag = new Tag();

            int firstIndex = s.IndexOf( '(' );
            int secondIndex = s.IndexOf( ')' );

            if ( firstIndex == -1 && secondIndex == -1 )
            {
                tag.Key = s.Substring( 1 );
            }

            else if ( firstIndex != -1 && secondIndex != -1 )
            {
                tag.Key = s.Substring( 1, firstIndex - 1 );
                tag.Values.AddRange(
                    s.Substring( firstIndex + 1, secondIndex - firstIndex - 1 )
                        .Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries )
                        .Select( x => x.Trim() ) );
            }

            else
            {
                return null;
            }

            return tag;
        }

        public static Tag Create( string key, params object[] values )
        {
            var tag = new Tag();
            tag.Key = key;

            foreach ( var value in values )
                tag.Values.Add( Convert.ToString( value, CultureInfo.InvariantCulture ) );

            return tag;
        }

        public static string GetName( string s )
        {
            int index = s.IndexOf( '@' );

            if ( index != -1 )
                return s.Substring( 0, index );

            return s;
        }

        public T GetValue<T>( int valueIndex = 0, T defaultValue = default )
        {
            if ( valueIndex < 0 || valueIndex >= Values.Count )
                return defaultValue;

            var converter = TypeDescriptor.GetConverter( typeof( T ) );
            if ( converter.CanConvertFrom( typeof( string ) ) )
                return ( T ) Convert.ChangeType( Values[ valueIndex ], typeof( T ), CultureInfo.InvariantCulture );

            return defaultValue;
        }

        public override string ToString()
        {
            if ( Values.Count <= 0 )
                return $"@{Key}";
            return $"@{Key}({string.Join( ",", Values )})";
        }

        public Tag()
        {
            Values = new List<string>();
        }
    }

    public class TagList : List<Tag>
    {
        public string Name { get; set; }

        public static TagList Parse( string s )
        {
            var tagList = new TagList();
            int index = s.IndexOf( '@' );

            if ( index == -1 )
            {
                tagList.Name = s;
            }

            else
            {
                tagList.Name = s.Substring( 0, index );

                while ( index != -1 )
                {
                    var tag = Tag.Parse( s.Substring( index ) );
                    if ( tag != null )
                        tagList.Add( tag );

                    index = s.IndexOf( '@', index + 1 );
                }
            }

            return tagList;
        }

        public T GetValue<T>( string key, int valueIndex = 0, T defaultValue = default )
        {
            var tag = this.FirstOrDefault( x =>
                x.Key.Equals( key, StringComparison.OrdinalIgnoreCase ) );

            if ( tag != null )
                return tag.GetValue( valueIndex, defaultValue );

            return defaultValue;
        }

        public override string ToString()
        {
            return $"{Name}{string.Join( "", this )}";
        }
    }
}