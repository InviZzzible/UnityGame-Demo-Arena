'use strict';

const express = require("express");
const app = express();
const http = require('http');
const port = 5555;
const mysql = require("mysql2");

const connection = mysql.createConnection({
  host: "localhost",
  user: "root",
  database: "unitygamedatabasezerobyte2d",
  password: "root"
});


app.get("/", (req, res) => {
    // ...
});

app.post("/", (req, res) => {
    let Data = "";
    req.on("data", data => {
        Data += data;
    });
    
    req.once("end", () => {
        if(Data){
            
            Data = Decipher(Data);
            let counter = 0;
            let arr = [];
            
            for(let i=0; i < Data.length; ++i){
                let Order = "";
                if(Data[i] != "|"){
                    arr[counter] += Data[i];
                }
                else{
                    counter++;
                }
            }
            
            if(arr[0] == "CkeckOnUniqNickname"){
                // Проверка уникальности никнейма среди БД:
                let Nick = arr[1];
                let sql = "SELECT COUNT(`Nickname`) FROM `persons` WHERE `Nickname`='" + Nick + "';";
                
                connection.query(sql, (err, results) => {
                    if(err){
                        console.log("Error: " + err.message);
                        connection.end();
                        res.end();
                    }
                    else{
                        if(sql == 0){ // Если совпадений в БД нет, тогда придуманный ник явл. уникальным
                            // Следовательно возвращаем true;
                            connection.end();
                            res.send(Encrypt("true"));
                        }
                        else{
                            connection.end();
                            res.send(Encrypt("false"));
                        }
                    }
                });
            }
            else if(arr[0] == "CheckOnExistingSuchNickname"){
                // Проверка существования уч. записи в БД:
                let Nick = arr[1];
                let Pass = arr[2];
                
                let check = CheckIn(Nick, Pass);
                
                if(check == "true"){
                    
                    let sql = "INSERT INTO `admin`(`Nickname`, `LastVisit`) VALUES ('" + Nick + "', '" + new Date().toString() + "');";
                        connection.query(sql, (err, results) => {
                            connection.end();
                            res.send(Encrypt("true"));
                        });
                }
                else if(check == "false"){
                    res.send(Encrypt("false"));
                }
                else if(check == "error"){
                    res.end();
                }
            }
            else if(arr[0] == "Create_character"){
                // Создание нового персонажа (Добавление данных в таблицу):
                let Nick = arr[1];
                let Pass = arr[2];
                let Model2D = arr[3];
                
                let sql = "INSERT INTO `persons`(`Nickname`, `Password`, `NumberOfModel`) " +
                            "VALUES ('" + Nick + "', '" + Pass + "', " + Model2D + "');\n" +
                    
                          "INSERT INTO `characters`(`Nickname`, `ForceOfCharacter`, `Health`, `MaxHealth`, `MaxMana`)" +
                            "VALUES ('" + Nick + "','5','100','100','100');\n" +
                    
                           "INSERT INTO `score`(`Nickname`, `PveCurrentLevel`, `PveCurrentScore`, `PveMaxScore`, `Gold`)" + 
                           "VALUES ('" + Nick + "','1','0','0','0');\n" +
                    
                           "INSERT INTO `admin`(`Nickname`, `LastVisit`) VALUES ('" + Nick + "', '" + new Date().toString() + "');\n" +
                    
                           "INSERT INTO `things`(`Nickname`, `Rustyknife`, `Ironknife`, `Steelknife`, `Titaniumknife`, `Diamondknife`, `Medicalherblvl1`, `Medicalherblvl2`, `Medicalherblvl3`, `Medicalherblvl4`, `Medicalherblvl5`, `Alchemistsset`, `Healthpotionlvl1`, `Healthpotionlvl2`, `Healthpotionlvl3`, `Potionofstrengthlvl1`, `Potionofstrengthlvl2`, `Potionofstrengthlvl3`, `Maxhealthpotionlvl1`, `Maxhealthpotionlvl2`, `Maxhealthpotionlvl3`) VALUES ('" + Nick + "','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0');";
                
                
                connection.query(sql, (err, results) => {
                    if(err){
                        console.log("Error: " + err.message);
                        connection.end();
                        res.end();
                    }
                    else{
                        connection.end();
                        res.send(Encrypt("true"));
                    }
                });
            }
            else if(arr[0] == "PveIndex"){ // Запрос индекса стартовой (актуальной) сцены
                let Nick = arr[1];
                let Pass = arr[2];
                
                let check = CheckIn(Nick, Pass);
                if(check == "true"){
                    let sql = "SELECT ``";
                    
                    connection.query(sql, (err, results) => {
                        if(err){
                            console.log("Error: " + err.message);
                            connection.end();
                            res.end();
                        }
                        else{
                            connection.end();
                            res.send("");
                        }
                    });
                    
                }
                else if(check == "false" || check == "error"){
                    res.end();
                }
            }
            else if(arr[0] == "PveTopDatas"){ // Запросить топ pve
                let Nick = arr[1];
                let Pass = arr[2];
                
                let check = CheckIn(Nick, Pass);
                if(check == "true"){
                    let sql = "";
                    
                    connection.query(sql, (err, results) => {
                        if(err){
                            console.log("Error: " + err.message);
                            connection.end();
                            res.end();
                        }
                        else{
                            connection.end();
                            res.send("");
                        }
                    });
                    
                }
                else if(check == "false" || check == "error"){
                    res.end();
                }
            }
            else if(arr[0] == "PvpTopDatas"){ // Запросить топ pvp
                let Nick = arr[1];
                let Pass = arr[2];
                
                let check = CheckIn(Nick, Pass);
                if(check == "true"){
                    let sql = "";
                    
                    connection.query(sql, (err, results) => {
                        if(err){
                            console.log("Error: " + err.message);
                            connection.end();
                            res.end();
                        }
                        else{
                            connection.end();
                            res.send("");
                        }
                    });
                    
                }
                else if(check == "false" || check == "error"){
                    res.end();
                }
            }
            else if(arr[0] == "CharactersInformation"){ // Запрашиваем информацию по персонажу
                let Nick = arr[1];
                let Pass = arr[2];
                
                let check = CheckIn(Nick, Pass);
                if(check == "true"){
                    let sql = "";
                    
                    connection.query(sql, (err, results) => {
                        if(err){
                            console.log("Error: " + err.message);
                            connection.end();
                            res.end();
                        }
                        else{
                            connection.end();
                            res.send("");
                        }
                    });
                    
                }
                else if(check == "false" || check == "error"){
                    res.end();
                }
            }
            else if(arr[0] == "StoreList"){ // Заправшиваем список всех товаров магазина
                let Nick = arr[1];
                let Pass = arr[2];
                
                let check = CheckIn(Nick, Pass);
                if(check == "true"){
                    let sql = "";
                    
                    connection.query(sql, (err, results) => {
                        if(err){
                            console.log("Error: " + err.message);
                            connection.end();
                            res.end();
                        }
                        else{
                            connection.end();
                            res.send("");
                        }
                    });
                    
                }
                else if(check == "false" || check == "error"){
                    res.end();
                }
            }
        }
        else res.end();
    });
    
});

app.listen(port);


// -------------- Functions:

function CheckIn(Nick, Pass){
    let sql = "SELECT `Password` FROM `persons` WHERE `Nickname`='" + Nick + "';";
    
    connection.query(sql, (err, results) => {
                    if(err){
                        console.log("Error: " + err.message);
                        connection.end();
                        return "error";
                    }
                    else{
                        if(sql != ""){
                            
                            if(sql == Pass){
                                connection.end();
                                return "true";
                            }
                            else{
                                connection.end();
                                return "false";
                            }
                        }
                        else{
                            connection.end();
                            return "false";
                        }
                    }
                });
}