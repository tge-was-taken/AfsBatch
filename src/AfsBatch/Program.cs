using System;
using System.IO;
using System.Threading.Tasks;
using PuyoTools.Modules.Archive;

namespace AfsBatch
{
    class Program
    {
        static void Main( string[] args )
        {
            if ( args.Length != 1 )
            {
                Console.WriteLine("AfsBatch <path to directory containing sub directories>");
                return;
            }
                
            string directoryPath = args[0];
            if ( !Directory.Exists(directoryPath) )
            {
                Console.WriteLine($"Directory \"{directoryPath}\" doesn't exist");
            }

            Parallel.ForEach( Directory.EnumerateDirectories( directoryPath ), ( string subDirectoryPath ) =>
            {
                var afsPath = subDirectoryPath + ".AFS";
                var afsWriter = new AfsArchiveWriter( File.Create( afsPath ) );

                afsWriter.BlockSize = 2048;
                afsWriter.HasTimestamps = true;
                afsWriter.Version = AfsArchiveWriter.AfsVersion.Version2;

                Console.WriteLine( $"Start writing {Path.GetFileName( afsPath )}" );
                foreach ( var filePath in Directory.EnumerateFiles( subDirectoryPath ) )
                {
                    var entryName = Path.GetFileName( filePath );
                    Console.WriteLine( $"Added entry {entryName}" );

                    afsWriter.CreateEntryFromFile( filePath, entryName );
                }

                afsWriter.Flush();
            } );

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
