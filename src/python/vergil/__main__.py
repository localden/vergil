import requests
import json
import subprocess
import sqlite3
from sqlite3 import Error

headers = {'Accept':'application/vnd.pypi.simple.v1+json'}

resp = requests.get("https://pypi.org/simple/", headers=headers)
data = json.loads(resp.content)

project_count = len(data['projects'])

print(f'Downloaded package feed. Identified {project_count} projects')

counter = 1

conn = sqlite3.connect('C:\\Users\\WDAGUtilityAccount\\Desktop\\packagedata.db')

for package in data['projects']:
    print(f'[{counter}/{project_count}] Downloading data for {package["name"]}')

    p_resp = requests.get(f'https://pypi.org/pypi/{package["name"]}/json')
    if p_resp.ok:
        try:           
            dep_process_result = subprocess.run(['johnnydep', package['name'], '--verbose', '0', '--ignore-errors', '--output-format', 'json'], stdout=subprocess.PIPE)
            query_result = dep_process_result.stdout
            try:
                json.loads(query_result)
            except:
                print(f'Could not obtain JSON list of dependencies. No dependencies will be registered for {package["name"]}')
                print(query_result)
                query_result = ''
        
            sql = ''' INSERT INTO PackageData(ResponseBody, Dependencies)
                      VALUES(?,?) '''
            cur = conn.cursor()
            cur.execute(sql, (p_resp.content, query_result))
            conn.commit()
            print(f'[{counter}/{project_count}] Stored data for {package["name"]}')
        except Error as e:
            print(f'[{counter}/{project_count}] FAILED STORAGE for {package["name"]} - {e}')   

    counter = counter + 1

print(query_result)
