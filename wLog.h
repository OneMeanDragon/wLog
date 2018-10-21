#pragma once

#include "Includes.h"
#include "sCritSect.h"

#define LOG_F "DATA\\LOG.TXT"

class wLog
{
private:
	SCritSect SCrit; //Critical section
	std::string m_logfile;

	bool File_Exists()
	{
		SCrit.enter();
		//Does this file exist?
		FILE *fp;
		errno_t err;
		err = fopen_s(&fp, m_logfile.c_str(), "r");
		if (err != NULL) {
			return false;
		}
		err = fclose(fp);
		SCrit.leave();
		return true;
	}
  
	bool Create_File()
	{
		SCrit.enter();
		FILE *fp;
		errno_t err;
		err = fopen_s(&fp, m_logfile.c_str(), "a");
		if (err != NULL)
		{
			return false;
		}
		fprintf(fp, "%s", "");
		err = fclose(fp);
		SCrit.leave();
		return true;
	}
  
	bool Create_Folder(const char folder_path[])
	{
		SCrit.enter();

		bool created = CreateDirectory(folder_path, NULL); //windows.h
		if (!created)
		{
			if (ERROR_ALREADY_EXISTS == GetLastError())
			{
				return true; //as the error says, the folder already exists good to go.
			}
			return false; //else we could not create this folder for (reasons)
		}

		SCrit.leave();
		return true;
	}

public:

	//Constructor will auto create our log text file if it dosent exist.
	wLog() : m_logfile(LOG_F)
	{
		if (!Create_Folder(".\\DATA\\")) { return; }
		if (!File_Exists())
		{
			if (!Create_File()) { return; }
		}
	}
	~wLog() {}

	void Write_Line(const char data[])
	{
		SCrit.enter();

		FILE *fs;
		fopen_s(&fs, m_logfile.c_str(), "at"); //
		fprintf_s(fs, "%s\r\n", data);
		fclose(fs);

		SCrit.leave();
	}
};
