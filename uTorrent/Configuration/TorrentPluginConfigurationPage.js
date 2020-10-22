define(["loading", "dialogHelper", "formDialogStyle"],
    function (loading, dialogHelper) {

        var pluginId = "b1390c15-5b4f-4038-bb58-b71b9ef4211b";
        
        var token = null;  

        var uTorrentProgressIntervalUpdate; //Only request torrent data when the page is shown on the screen

        function openAddTorrentDialog() {
            loading.show();

            var dlg = dialogHelper.createDialog({
                size: "medium-tall",
                removeOnClose: !1,
                scrollY: true
            });

            dlg.classList.add("formDialog");
            dlg.classList.add("ui-body-a");
            dlg.classList.add("background-theme-a");
            dlg.style.maxWidth = "25%";
            dlg.style.maxHeight = "50%";

            var html = '';
            html += '<div class="formDialogHeader" style="display:flex">';
            html += '<button is="paper-icon-button-light" class="btnCloseDialog autoSize paper-icon-button-light" tabindex="-1"><i class="md-icon">arrow_back</i></button><h3 class="formDialogHeaderTitle">Add torrent from url</h3>';
            html += '</div>';

            html += '<div class="formDialogContent" style="margin:2em">';
            html += '<div class="dialogContentInner" style="max-height: 42em;">';
            html += '<div style="flex-grow:1;">';

            html += '<h2>Torrent Url</h2>';
            html += '<input id="torrentUrl" is="emby-input" />';

            html += '</div>';

            html += '<div class="formDialogFooter" style="padding-top:2.5em">';
            html += '<button id="submitUrl" class="raised button-submit block emby-button" is="emby-button">Ok</button>';
            html += '</div>';

            html += '</div>';
            html += '</div>';

            dlg.innerHTML = html;

            dlg.querySelector('#submitUrl').addEventListener('click',
                () => {
                    ApiClient.getPluginConfiguration(pluginId).then((config) => {
                        if (!config.userName) return;
                        addTorrentUrl(dlg.querySelector('#torrentUrl').value, config).then((result) => {
                            if (result == "OK") {
                                Dashboard.alert({
                                    title: "Torrent Added Successfully",
                                    message: ""
                                });
                            }
                            dialogHelper.close(dlg);
                        });
                    });
                });

            dlg.querySelector('.btnCloseDialog').addEventListener('click',
                () => {
                    dialogHelper.close(dlg);
                });

            dialogHelper.open(dlg);
            loading.hide();

        }

        function openSettingsDialog(view) {
            loading.show();

            var dlg = dialogHelper.createDialog({
                size          : "medium-tall",
                removeOnClose : !1,
                scrollY       : true
            });

            dlg.classList.add("formDialog");
            dlg.classList.add("ui-body-a");
            dlg.classList.add("background-theme-a");
            dlg.style = "margin:2em; max-width: 70em; max-height:38em";

            var html = '';
            html += '<div class="formDialogHeader" style="display:flex">';
            html += '<button is="paper-icon-button-light" class="btnCloseDialog autoSize paper-icon-button-light" tabindex="-1"><i class="md-icon">arrow_back</i></button><h3 class="formDialogHeaderTitle">uTorrent Credentials</h3>';
            html += '</div>';

            html += '<div class="formDialogContent">';
            html += '<div class="dialogContentInner">';

            html += '<div style="flex-grow:1;margin:3em">';

            html += '<label for="ip">Ip Address</label>';
            html += '<input id="ip" name="ip" class="emby-input" />';

            html += '<label for="port">Port</label>';
            html += '<input id="port" name="port" is="emby-input" />';

            html += '<label for="name">User Name</label>';
            html += '<input id="user" name="name" class="emby-input" />';

            html += '<label for="pass">Password</label>';
            html += '<input id="pass" name="pass" type="password" class="emby-input" />';

            html += '<label for="finishedDownloadLocation">Finished Download Location</label>';
            html += '<input id="finishedDownloadLocation" name="finishedDownloadLocation" class="emby-input"/>';
             
            html += '<button id="saveButton" class="raised button-submit block emby-button" style="width:50%; margin:auto; margin-top:2em" is="emby-button">Save</button>';

            html += '</div>';
            html += '</div>';

            dlg.innerHTML = html;

            
            loading.hide();
            dialogHelper.open(dlg);
            

            dlg.querySelector('#saveButton').addEventListener('click',
                () => {
                    var config = {
                        userName                  : dlg.querySelector('#user').value,
                        password                  : dlg.querySelector('#pass').value,
                        ipAddress                 : dlg.querySelector('#ip').value,
                        port                      : dlg.querySelector('#port').value,
                        FinishedDownloadsLocation : dlg.querySelector('#finishedDownloadLocation').value 
                    }

                    ApiClient.updatePluginConfiguration(pluginId, config).then(function () {

                        loadPageData(view, config); 

                        //ApiClient.getJSON(ApiClient.getUrl("uTorrentPluginDataFolderPath")).then(folder => {
                        //    setSettings("state_cmd", folder.path + "/uTorrentWebSocketMessenger.exe http://192.168.2.126:8096/emby/SendUpdateMessage", config).then(r => {
                        //        Dashboard.processPluginConfigurationUpdateResult(r);
                        //    });
                        //});

                    });

                    dialogHelper.close(dlg);
                });

            dlg.querySelector('.btnCloseDialog').addEventListener('click',
                () => {
                    dialogHelper.close(dlg);
                }); 
            ApiClient.getPluginConfiguration(pluginId).then(
                (config) => {
                    if (config.userName) {
                        dlg.querySelector('#user').value                     = config.userName;
                        dlg.querySelector('#pass').value                     = config.password;
                        dlg.querySelector('#ip').value                       = config.ipAddress;
                        dlg.querySelector('#port').value                     = config.port;
                        dlg.querySelector('#finishedDownloadLocation').value = config.FinishedDownloadsLocation;
                    }
                    
                });
            
        }

        function getTorrentResultTableHtml(torrents) {
            var html = '';
            torrents.forEach(torrent => {

                html += '<tr class="detailTableBodyRow detailTableBodyRow-shaded" id="' + torrent.Hash + '">'; 
                html += '<td class="detailTableBodyCell" data-title="Status">';
                html += '<i class="md-icon"';

                if (torrent.Progress == 1000) {

                    html += 'style="color:green; font-size:2em">check';

                } else {

                    switch (torrent.Status) {
                        case 'started' : html += 'style="color:#4584b5; font-size:2em; font-weight:light; cursor:default">play_arrow';
                            break;
                        case 'stopped' : html += 'style="color:red; font-size:2em; font-weight:light;cursor:default">stop';
                            break;
                        case 'paused'  : html += 'style="color:orange; font-size:2em; font-weight:light;cursor:default">paused';
                            break;
                        case 're-check': html += 'style="color:orange; font-size:2em; font-weight:light;cursor:default">sync';
                            break;
                        case "queued"  : html += 'style="color:#4584b5; font-size:2em; font-weight:light;cursor:default">restore';
                            break;
                    }

                }

                html += '</i></td>';
                html += '<td data-title="Name" class="detailTableBodyCell fileCell">' + torrent.Name + '</td>';
                html += '<td data-title="Size" class="detailTableBodyCell fileCell">' + torrent.Size + '</td>';
                html += '<td data-title="Speed" class="detailTableBodyCell fileCell">' + torrent.DownloadSpeedFriendly + '/s</td>';
                html += '<td data-title="Progress" class="detailTableBodyCell fileCell">';
                html += '<div style="display:flex;align-items:center;">';
                html += '<div class="taskProgressOuter" title="' + (torrent.Progress / 10) + '%" style="flex-grow:1;">';
                html += '<div class="taskProgressInner" style="width:' + (torrent.Progress / 10) + '%; height:0.3em">';
                html += '</div>';
                html += '</div>';
                html += '</div>';
                html += '</td>';
                html += '<td data-title="Complete" class="detailTableBodyCell fileCell"><span>' + (torrent.Progress / 10) + '%</span></td>';
                html += '<td data-title="Eta" class="detailTableBodyCell fileCell">' + (torrent.Status == "queued" ? "Queued" : torrent.Eta) + '</td>';
                html += '<td data-title="Date Added" class="detailTableBodyCell fileCell">' + torrent.AddedDate + '</td>';
                html += '<td data-title="Remove" class="detailTableBodyCell fileCell">';
                html += '<button id="' + torrent.Hash + '" class="fab removeTorrent emby-button"><i class="md-icon">clear</i></button>';
                html += '</td>';

                html += '<td class="detailTableBodyCell organizerButtonCell" style="whitespace:no-wrap;"></td>';

                html += '</tr>';
            });

            return html;
        }

        function getToken(config) {
            return new Promise((resolve, reject) => {
                if (config.userName) {
                    ApiClient.getJSON(ApiClient.getUrl("GetToken")).then((result) => {
                            resolve(result.token);
                        });
                }
            });
        }

        function setSettings(setting, value, config) {
            return new Promise((resolve, reject) => {
                if (token == null) {
                    getToken(config).then(t => {
                        token = t;
                        ApiClient.getJSON(ApiClient.getUrl("SetSettingsData?Token=" +
                            token +
                            "&SettingName=" +
                            setting +
                            "&SettingValue=" +
                            value)).then((result) => {
                                resolve(result);
                            });
                    });

                } else {

                    ApiClient.getJSON(ApiClient.getUrl("SetSettingsData?Token=" +
                        token + 
                        "&SettingName=" +
                        setting +
                        "&SettingValue=" +
                        value)).then((settingsData) => {
                            resolve(settingsData);
                        },
                        () => {
                            getToken(config).then(t => {
                                token = t;
                                ApiClient.getJSON(ApiClient.getUrl("SetSettingsData?Token=" +
                                    encodeURIComponent(token) + 
                                    "&SettingName=" +
                                    setting +
                                    "&SettingValue=" +
                                    value)).then((settingsData) => {
                                        resolve(settingsData);
                                    });
                            });
                        });
                }
            });
        }

        function getSettings(config) {
            return new Promise((resolve, reject) => {
                if (token == null) {
                    getToken(config).then(t => {
                        token = t;
                        ApiClient.getJSON(ApiClient.getUrl("GetSettingsData?Token=" +
                           token)).then((settingsData) => {
                                resolve(settingsData);
                            });
                    });

                } else {

                    ApiClient.getJSON(ApiClient.getUrl("GetSettingsData?Token=" +
                        token)).then((settingsData) => {
                            resolve(settingsData);
                        },
                            () => {
                                getToken(config).then(t => {
                                    token = t;
                                    ApiClient.getJSON(ApiClient.getUrl("GetSettingsData?Token=" +
                                        encodeURIComponent(token))).then((settingsData) => {
                                            resolve(settingsData);
                                        });
                                });
                            });
                }
            });
        }
        
        function removeTorrent(hash, config) {
            return new Promise((resolve, reject) => {
                if (token == null) {
                    getToken(config).then(t => {
                        token = t;
                        ApiClient.getJSON(ApiClient.getUrl("RemoveTorrent?Token=" +
                            token + 
                            "&Id=" + hash)).then((result) => {
                                resolve(result);
                            });
                    });

                } else {

                    ApiClient.getJSON(ApiClient.getUrl("RemoveTorrent?Token=" +
                        token + 
                        "&Id=" + hash)).then((result) => {
                            resolve(result);
                        },
                            () => {
                                getToken(config).then(t => {
                                    token = t;
                                    ApiClient.getJSON(ApiClient.getUrl("RemoveTorrent?Token=" +
                                        token + 
                                        "&Id=" + hash)).then((result) => {
                                            resolve(result);
                                        });
                                });
                            });
                }
            });
        }
        
        function getUTorrentData(config, sortBy) {
            return new Promise((resolve, reject) => {
                if (token == null) {
                    getToken(config).then(t => {
                        token = t;
                        ApiClient.getJSON(ApiClient.getUrl("GetTorrentData?Token=" +
                            token + 
                            "&SortBy=" +
                            sortBy)).then((torrentData) => {
                            resolve(torrentData);
                        });
                    });

                } else {

                    ApiClient.getJSON(ApiClient.getUrl("GetTorrentData?Token=" +
                        token +
                        "&SortBy=" +
                        sortBy)).then((torrentData) => { 
                            resolve(torrentData);
                        },
                        () => {
                            getToken(config).then(t => {
                                token = t;
                                ApiClient.getJSON(ApiClient.getUrl("GetTorrentData?Token=" +
                                    token +
                                    "&SortBy=" +
                                    sortBy)).then((torrentData) => {
                                    resolve(torrentData);
                                });
                            });
                        });
                }
            });
        }
           
        function remoteControlTorrent(remoteCommand, id, config) {
            return new Promise((resolve, reject) => {
                getToken(config).then((token) => {
                    ApiClient.getJSON(ApiClient.getUrl(
                        remoteCommand +
                        "?Token=" +
                        encodeURIComponent(token) +
                        "&IpAddress=" +
                        config.ipAddress +
                        "&Port=" +
                        config.port +
                        "&UserName=" +
                        encodeURIComponent(config.userName) +
                        "&Password=" +
                        encodeURIComponent(config.password) +
                        "&Id=" +
                        id)).then((response) => {
                            resolve(response.status);
                        });
                });
            });
        }

        function addTorrentUrl(url, config) {
            return new Promise((resolve, reject) => {
                getToken(config).then((token) => {
                    ApiClient.getJSON(ApiClient.getUrl("AddTorrentUrl?Token=" +
                        token +
                        "&Url=" +
                        encodeURIComponent(url))).then((result) => {
                            resolve(result.status);
                        });
                });
            });
        }

        function loadDialogData(dlg, config) {
            if (config.userName) {

                dlg.querySelector('#user').value                     = config.userName;
                dlg.querySelector('#pass').value                     = config.password;
                dlg.querySelector('#ip').value                       = config.ipAddress;
                dlg.querySelector('#port').value                     = config.port;
                dlg.querySelector('#finishedDownloadLocation').value = config.FinishedDownloadsLocation; 

            }

        }

        function loadSelectNumericOptions(increments, total) {
            var html = '';
            html += '<option value="0">Unlimited</option>';
            for (var i = increments; i <= total; i += increments) {
                html += '<option value="' + i + '">' + i + '</option>';
            }
            return html;
        }

        function updateTorrentResultTable(view, config) {
            if (uTorrentProgressIntervalUpdate) {
                //    setTimeout(() => {
                var sortBySelect = view.querySelector('#selectSortListBy');
                var sort = sortBySelect.value;
                getUTorrentData(config, sort).then((results) => {
                    view.querySelector('.torrentResultBody').innerHTML = getTorrentResultTableHtml(results.torrents);
                    view.querySelector('.torrentInfoContainer > h2').innerText = results.TotalRecordCount + ' torrents';
                    view.querySelectorAll('.removeTorrent').forEach(removeTorrentButton => {
                        removeTorrentButton.addEventListener('click',
                            (e) => {
                                var tableRow = e.target.closest('tr');
                                tableRow.querySelector('.taskProgressInner').style = "background-color:yellow";
                                tableRow.disabled = true;
                                removeTorrent(e.target.closest('button').id, config).then(result => {
                                    console.log(result.status);
                                });
                            });
                    });
                    console.log("uTorrent data updated");
                    Dashboard.hideLoadingMsg();
                    updateTorrentResultTable(view, config);
                });
                
                //    }, 1000);
            }
        }
        
        function loadPageData(view, config) {
            if (config.userName) {
                
                getSettings(config).then(results => {
                    var settings = results.settings;

                    console.log(settings[26]);  //UP
                    console.log(settings[25]); //DL

                    var activeDownloads = view.querySelector('#selectNumActiveDownloads');
                    var activeTorrents  = view.querySelector('#selectNumActiveTorrents');
                    var upload          = view.querySelector('#selectMaxUpload');
                    var download        = view.querySelector('#selectMaxDownload');
                     
                    upload.innerHTML          = loadSelectNumericOptions(5, 1000);
                    download.innerHTML        = loadSelectNumericOptions(5, 1000);
                    activeDownloads.innerHTML = loadSelectNumericOptions(1, 50);
                    activeTorrents.innerHTML  = loadSelectNumericOptions(1, 50);
                    
                    var settingsDownloadSpeed         = settings[25];
                    var settingsUploadSpeed           = settings[26];
                    var settingsActiveDownloadCount   = settings[52];
                    var settingsActiveTorrentsCount   = settings[51];
                    var stateCommand                  = settings[72];

                    console.log(stateCommand[2]);

                    download.value        = settingsDownloadSpeed[2];
                    upload.value          = settingsUploadSpeed[2];
                    activeDownloads.value = settingsActiveDownloadCount[2];
                    activeTorrents.value  = settingsActiveTorrentsCount[2];
                    uTorrentProgressIntervalUpdate = true;
                    updateTorrentResultTable(view, config); 

                });




                view.querySelector('#selectMaxUpload').addEventListener('change', 
                    (e) => {
                    setSettings("max_ul_rate", e.target.value, config).then((result) => {
                        console.log("max_ul_rate " + e.target.value);
                        console.log(result.status);
                    });
                });

                view.querySelector('#selectMaxDownload').addEventListener('change',
                    (e) => {
                        setSettings("max_dl_rate", e.target.value, config).then((result) => {
                            console.log("max_dl_rate " + e.target.value);
                            console.log(result.status);
                        });
                    });

                view.querySelector('#selectNumActiveDownloads').addEventListener('change', 
                    (e) => {
                        setSettings("max_active_downloads", e.target.value, config).then((result) => {
                            console.log("max_active_downloads " + e.target.value);
                            console.log(result.status);
                        });
                    });

                view.querySelector('#selectNumActiveTorrents').addEventListener('change',
                    (e) => {
                        setSettings("max_active_torrent", e.target.value, config).then((result) => {
                            console.log("max_active_torrent " + e.target.value); 
                            console.log(result.status);
                        });
                    });

                //ApiClient._webSocket.addEventListener('message',
                //    function(message) {
                //        var json = JSON.parse(message.data);
                //        if (json.MessageType === "UpdateList") {
                //            updateTorrentResultTable(view, config);
                //        }
                //    });


                    Dashboard.hideLoadingMsg();
            }  
        }

        function loadConfig(view) {
            ApiClient.getPluginConfiguration(pluginId).then(
                (config) => {
                    if (view.classList.contains('dialog')) {
                        loadDialogData(view, config);
                        return;
                    }
                    loadPageData(view, config);
                });
        }
         
        return function (view) {
            view.addEventListener('viewshow',
                () => {
                    Dashboard.showLoadingMsg(); 
                    token = null; 

                    loadConfig(view);

                    view.querySelector('#openTorrentDialog').addEventListener('click',
                        (e) => {
                            e.preventDefault();
                            loading.show();
                            openSettingsDialog(view); 
                        });

                    view.querySelector('#openAddTorrentDialog').addEventListener('click',
                        (e) => {
                            e.preventDefault();
                            openAddTorrentDialog();
                        });   
                   
                    view.querySelector('#selectSortListBy').addEventListener('change', () => {
                        Dashboard.showLoadingMsg();
                    });
                });

            view.addEventListener('viewhide', () => {
                uTorrentProgressIntervalUpdate = false;
            });

            view.addEventListener('viewdestroy', () => {
                uTorrentProgressIntervalUpdate = false;
            });

        }
    });