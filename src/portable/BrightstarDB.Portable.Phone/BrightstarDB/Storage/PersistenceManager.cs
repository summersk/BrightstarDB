﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using FileMode = BrightstarDB.Portable.Compatibility.FileMode;

namespace BrightstarDB.Storage
{
    public class PersistenceManager : IPersistenceManager
    {
        private IsolatedStorageFile _isolatedStorage;

        public PersistenceManager()
        {
            _isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
        }

        #region Implementation of IPersistenceManager

        /// <summary>
        /// Returns true if <paramref name="pathName"/> represents
        /// the path to an existing "file" in the persistence store
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public bool FileExists(string pathName)
        {
            return _isolatedStorage.FileExists(pathName);
        }

        /// <summary>
        /// Create a new empty file at the specified path location
        /// </summary>
        /// <param name="pathName"></param>
        public void CreateFile(string pathName)
        {
            _isolatedStorage.CreateFile(pathName).Close();
        }

        /// <summary>
        /// Removes a file from the persistence store
        /// </summary>
        /// <param name="pathName"></param>
        public void DeleteFile(string pathName)
        {
            if (_isolatedStorage.FileExists(pathName))
            {
                _isolatedStorage.DeleteFile(pathName);
            }
        }

        /// <summary>
        /// Returns true if <paramref name="pathName"/> represents
        /// the path to an existing "directory" in the persistence store
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public bool DirectoryExists(string pathName)
        {
            return _isolatedStorage.DirectoryExists(pathName);
        }

        /// <summary>
        /// Creates a new directory in the persistence store.
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public void CreateDirectory(string dirName)
        {
            _isolatedStorage.CreateDirectory(dirName);
        }

        /// <summary>
        /// Removes a directory and any files or subdirectories within it
        /// from the persistence store
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public void DeleteDirectory(string dirName)
        {
            foreach(var fileName in _isolatedStorage.GetFileNames(dirName + Path.DirectorySeparatorChar + "*"))
            {
                DeleteFile(Path.Combine(dirName, fileName));
            }
            _isolatedStorage.DeleteDirectory(dirName);
        }

        /// <summary>
        /// Opens a stream to write to a file in the persistence store
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="mode">The mode used to open the file</param>
        /// <returns></returns>
        public Stream GetOutputStream(string pathName, FileMode mode)
        {
            switch (mode)
            {
                case FileMode.Append:
                    return _isolatedStorage.OpenFile(pathName, System.IO.FileMode.Append, FileAccess.Write, FileShare.Read);
                case FileMode.Create:
                    return _isolatedStorage.OpenFile(pathName, System.IO.FileMode.Create, FileAccess.Write, FileShare.Read);
                case FileMode.CreateNew:
                    return _isolatedStorage.OpenFile(pathName, System.IO.FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                case FileMode.Open:
                    return _isolatedStorage.OpenFile(pathName, System.IO.FileMode.Open, FileAccess.Write, FileShare.Read);
                case FileMode.OpenOrCreate:
                    return _isolatedStorage.OpenFile(pathName, System.IO.FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                case FileMode.Truncate:
                    return _isolatedStorage.OpenFile(pathName, System.IO.FileMode.Truncate, FileAccess.Write, FileShare.Read);
                default:
                    throw new ArgumentException("Invalid file mode");
            }
        }

        /// <summary>
        /// Opens a stream to read from a file in the persistence store
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public Stream GetInputStream(string pathName)
        {
            return _isolatedStorage.OpenFile(pathName, System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public long GetFileLength(string pathName)
        {
            if (_isolatedStorage.FileExists(pathName))
            {
                var file = _isolatedStorage.OpenFile(pathName, System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var length = file.Length;
                file.Close();
                return length;
            }
            return 0;
        }

        public IEnumerable<string> ListSubDirectories(string dirName)
        {
            if(!_isolatedStorage.DirectoryExists(dirName)) throw new FileNotFoundException("Cannot find directory '{0}'", dirName);
            return _isolatedStorage.GetDirectoryNames(dirName+"\\*");
        }

        public void RenameFile(string storeConsolidateFile, string storeDataFile)
        {
           _isolatedStorage.MoveFile(storeConsolidateFile, storeDataFile);
        }

        public void CopyFile(string sourceFilePath, string destinationFilePath, bool overwrite)
        {
            _isolatedStorage.CopyFile(sourceFilePath, destinationFilePath, overwrite);
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_isolatedStorage !=null)
            {
                _isolatedStorage.Dispose();
                _isolatedStorage = null;
            }
        }

        #endregion
    }
}
