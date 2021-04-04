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
                        res.end;
                    }
                    else{
                        if(sql == 0){ // Если совпадений в БД нет, тогда придуманный ник явл. уникальным
                            // Следовательно возвращаем true;
                            connection.end();
                            res.send("true");
                        }
                        else{
                            connection.end();
                            res.send("false");
                        }
                    }
                });
            }
            else if(arr[0] == "CheckOnExistingSuchNickname"){
                // Проверка существования уч. записи в БД:
                let Nick = arr[1];
                let Pass = arr[2];
                let sql = "SELECT `Password` FROM `persons` WHERE `Nickname`='" + Nick + "';";
                
                connection.query(sql, (err, results) => {
                    if(err){
                        console.log("Error: " + err.message);
                        connection.end();
                        res.end;
                    }
                    else{
                        if(sql != ""){
                            
                            if(sql == Pass){
                                connection.end();
                                res.send("true");
                            }
                            else{
                                connection.end();
                                res.send("false");
                            }
                        }
                        else{
                            connection.end();
                            res.send("false");
                        }
                    }
                });
            }
            else if(arr[0] == "Create_character"){
                // Создание нового персонажа (Добавление данных в таблицу):
                let Nick = arr[1];
                let Pass = arr[2];
                let Model2D = arr[3];
                
                let sql = "INSERT INTO `persons`(`Nickname`, `Password`, `NumberOfModel`) " +
                            "VALUES ('" + Nick + "', '" + Pass + "', " + Model2D + "');";
                
                connection.query(sql, (err, results) => {
                    if(err){
                        console.log("Error: " + err.message);
                        connection.end();
                        res.end();
                    }
                    else{
                        connection.end();
                        res.send("true");
                    }
                });
            }
            // ...
        }
        else res.end();
    });
    
});

app.listen(port);