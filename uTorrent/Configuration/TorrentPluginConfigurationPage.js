define(["loading", "dialogHelper"],
    function (loading, dialogHelper) {

        var pluginId = "b1390c15-5b4f-4038-bb58-b71b9ef4211b";

        var torrentMonitor;  //Interval Timer to update torrent data for real time monitoring every 5 seconds
        var realTimeMonitor; //Interval Timer to update download speed chart data for real time monitoring every 1.5 seconds


        function calculateTotalTorrentDriveSpace(config) {
            return new Promise((resolve, reject) => {
                getToken(config).then(token => {
                    ApiClient.getJSON(ApiClient.getUrl("GetTotalDriveSpaceUsed?Token=" +
                        token +
                        "&IpAddress=" +
                        config.ipAddress +
                        "&Port=" +
                        config.port +
                        "&UserName=" +
                        encodeURIComponent(config.userName) +
                        "&Password=" +
                        encodeURIComponent(config.password))).then((result) => {
                            resolve(result);
                        });
                });
            });
        }

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
                        addTorrentUrl(dlg.querySelector('#torrentUrl').value, config).then(() => {
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
                size: "medium-tall",
                removeOnClose: !1,
                scrollY: true
            });

            dlg.classList.add("formDialog");
            dlg.classList.add("ui-body-a");
            dlg.classList.add("background-theme-a");
            dlg.style = "margin:2em; max-width: 25em; max-height:22em";

            var html = '';
            html += '<div class="formDialogHeader" style="display:flex">';
            html += '<button is="paper-icon-button-light" class="btnCloseDialog autoSize paper-icon-button-light" tabindex="-1"><i class="md-icon">arrow_back</i></button><h3 class="formDialogHeaderTitle">uTorrent Credentials</h3>';
            html += '</div>';

            html += '<div class="formDialogContent">';
            html += '<div class="dialogContentInner">';

            html += '<div style="flex-grow:1;margin:3em">';

            html += '<label for="ip">Ip Address</label>';
            html += '<input id="ip" name="ip" is="emby-input" />';

            html += '<label for="port">Port</label>';
            html += '<input id="port" name="port" is="emby-input" />';

            html += '<label for="name">User Name</label>';
            html += '<input id="user" name="name" is="emby-input" />';

            html += '<label for="pass">Password</label>';
            html += '<input id="pass" name="pass" type="password" is="emby-input" />';

            html += '<label for="finishedDownloadLocation">Finished Download Location</label>';
            html += '<input id="finishedDownloadLocation" name="finishedDownloadLocation" is="emby-input" />';


            html += '<div class="formDialogFooter" style="padding-top:2.5em">';
            html += '<button id="saveButton" class="raised button-submit block emby-button" style="width:50%; margin:auto;" is="emby-button">Save</button>';

            html += '</div>';
            html += '</div>';
            html += '</div>';
            dlg.innerHTML = html;

            loadConfig(dlg);

            dlg.querySelector('#saveButton').addEventListener('click',
                () => {
                    var config = {
                        userName: dlg.querySelector('#user').value,
                        password: dlg.querySelector('#pass').value,
                        ipAddress: dlg.querySelector('#ip').value,
                        port: dlg.querySelector('#port').value,
                        FinishedDownloadsLocation: dlg.querySelector('#finishedDownloadLocation').value,

                    }

                    ApiClient.updatePluginConfiguration(pluginId, config).then(function (result) {

                        loadPageData(view, config);

                        Dashboard.processPluginConfigurationUpdateResult(result);
                    });

                    dialogHelper.close(dlg);
                });

            dlg.querySelector('.btnCloseDialog').addEventListener('click',
                () => {
                    dialogHelper.close(dlg);
                });

            dialogHelper.open(dlg);
            loading.hide();
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
                        case 'started':
                            html += 'style="color:#4584b5; font-size:2em; font-weight:light; cursor:default">play_arrow';
                            break;
                        case 'stopped':
                            html += 'style="color:red; font-size:2em; font-weight:light;cursor:default">stop';
                            break;
                        case 'paused':
                            html += 'style="color:orange; font-size:2em; font-weight:light;cursor:default">paused';
                            break;
                        case 're-check':
                            html += 'style="color:orange; font-size:2em; font-weight:light;cursor:default">sync';
                            break;
                        case "queued":
                            html += 'style="color:#4584b5; font-size:2em; font-weight:light;cursor:default">restore';
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
                html += '<td data-title="Eta" class="detailTableBodyCell fileCell">' + torrent.Eta + '</td>';
                html += '<td data-title="Date Added" class="detailTableBodyCell fileCell">' + torrent.AddedDate + '</td>';
                html += '<td class="detailTableBodyCell organizerButtonCell" style="whitespace:no-wrap;"></td>';
                html += '</tr>';
            });
            return html;
        }

        function getToken(config) {
            return new Promise((resolve, reject) => {
                if (config.userName) {
                    ApiClient.getJSON(ApiClient.getUrl("GetToken?UserName=" +
                        encodeURIComponent(config.userName) +
                        "&Password=" +
                        encodeURIComponent(config.password) +
                        "&IpAddress=" +
                        config.ipAddress +
                        "&Port=" +
                        config.port)).then((result) => {
                            resolve(result.token);
                        });
                }
            });
        }

        function getTorrents(config, sortBy) {
            return new Promise((resolve, reject) => {
                getToken(config).then(token => {
                    ApiClient.getJSON(ApiClient.getUrl("GetFiles?Token=" +
                        encodeURIComponent(token) +
                        "&IpAddress=" +
                        config.ipAddress +
                        "&Port=" +
                        config.port +
                        "&UserName=" +
                        encodeURIComponent(config.userName) +
                        "&Password=" +
                        encodeURIComponent(config.password) +
                        "&SortBy=" + sortBy)).then((torrents) => {
                            resolve(torrents);
                        });
                });
            });
        }

        function getDownloadingTorrents(config) {
            return new Promise((resolve, reject) => {
                getToken(config).then((token) => {
                    ApiClient.getJSON(ApiClient.getUrl("GetDownloads?Token=" +
                        encodeURIComponent(token) +
                        "&IpAddress=" +
                        config.ipAddress +
                        "&Port=" +
                        config.port +
                        "&UserName=" +
                        encodeURIComponent(config.userName) +
                        "&Password=" +
                        encodeURIComponent(config.password))).then((torrents) => {
                            resolve(torrents);
                        });
                });
            });
        }

        function getDownloadRate(config) {
            return new Promise((resolve, reject) => {
                getToken(config).then((token) => {
                    ApiClient.getJSON(ApiClient.getUrl("GetDownloadRate?Token=" +
                        encodeURIComponent(token) +
                        "&IpAddress=" +
                        config.ipAddress +
                        "&Port=" +
                        config.port +
                        "&UserName=" +
                        encodeURIComponent(config.userName) +
                        "&Password=" +
                        encodeURIComponent(config.password))).then((result) => {
                            resolve(result);
                        });
                });
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
                        "&IpAddress=" +
                        config.ipAddress +
                        "&Port=" +
                        config.port +
                        "&UserName=" +
                        encodeURIComponent(config.userName) +
                        "&Password=" +
                        encodeURIComponent(config.password) +
                        "&Url=" +
                        url)).then((result) => {
                            resolve(result.status);
                        });
                });
            });
        }

        function loadDialogData(dlg, config) {
            if (config.userName) {
                dlg.querySelector('#user').value = config.userName;
                dlg.querySelector('#pass').value = config.password;
                dlg.querySelector('#ip').value = config.ipAddress;
                dlg.querySelector('#port').value = config.port;
                dlg.querySelector('#finishedDownloadLocation').value = config.FinishedDownloadsLocation;

            }

        }

        
        function updateTorrentData(chartData, chartLabels, c, view) {
            ApiClient.getPluginConfiguration(pluginId).then(
                (config) => { 
                    getDownloadRate(config).then(
                        (result) => {
                            if (chartData.length > 5) {
                                chartData.splice(0, 1);
                                chartLabels.splice(0, 1);
                            }
                            c.data.datasets[0].data.push(parseInt(result.size, 10));
                            c.data.labels.push(new Date().getSeconds());
                            c.data.datasets[0].label = "Download Speed (" + result.sizeSuffix + ")";
                            c.update();

                            getTorrents(config, "DateAdded").then(
                                (torrents) => {
                                    view.querySelector('.torrentResultBody').innerHTML = getTorrentResultTableHtml(torrents);
                                    calculateTotalTorrentDriveSpace(config).then((totalSize) => {
                                        view.querySelector('#torrentListHeader').innerHTML =
                                            "Torrents By Date Added: " + totalSize.size;
                                        if (realTimeMonitor === true) {
                                            setTimeout(updateTorrentData(chartData, chartLabels, c, view), 2000);
                                        }
                                    });
                                });
                        });
                });
        }

        function enableRealTimeMonitoring(view) {
            require([Dashboard.getConfigurationResourceUrl('Chart.bundle.js')],
                (chart) => {
                    var c = drawDownloadChart(view, chart);
                    var chartData = c.data.datasets[0].data;
                    var chartLabels = c.data.labels;
                    updateTorrentData(chartData, chartLabels, c, view);
                    loading.hide();
                });
        }

        function loadPageData(view, config) {
            if (config.userName) {
                if (config.EnableRealtimeMonitoring) {
                    enableRealTimeMonitoring(view);
                    view.querySelector('#enableRealTimeMonitoring').checked = config.EnableRealtimeMonitoring;
                    realTimeMonitor = config.EnableRealtimeMonitoring;
                    return;
                } else {
                    getTorrents(config, "DateAdded").then((torrents) => {
                        view.querySelector('.torrentResultBody').innerHTML = getTorrentResultTableHtml(torrents);

                    });
                    calculateTotalTorrentDriveSpace(config).then(totalSize => {
                        view.querySelector('#torrentListHeader').innerHTML = "Torrents By Date Added: " + totalSize.size;
                    });
                    loading.hide();
                    return;
                }
            }
            loading.hide();
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

        function tableItemClick(table, config) {
            table.querySelectorAll('i.md-icon').forEach(item => {

                item.addEventListener('click', (e) => {
                    clearInterval(torrentMonitor);
                    var id = e.target.closest('.detailTableBodyRow').id;

                    getDownloadingTorrents(config).then((torrents) => {

                        torrents.forEach(torrent => {

                            if (torrent.Hash !== id) return;

                            switch (torrent.Status) {
                                case "started":
                                    remoteControlTorrent("StopTorrent", id, config).then(
                                        (response) => {
                                            console.log("torrent stopped " + response.status);
                                        });

                                    torrentMonitor = setInterval(() => {
                                        getDownloadingTorrents(config).then((torrents) => {
                                            table.innerHTML =
                                                getTorrentResultTableHtml(torrents);
                                        });
                                    },
                                        5000);
                                    break;

                                case "stopped":
                                    remoteControlTorrent("StartTorrent", id, config).then(
                                        (response) => {
                                            console.log("torrent started " + response.status);
                                        });
                                    torrentMonitor = setInterval(() => {
                                        getDownloadingTorrents(config).then((torrents) => {
                                            table.innerHTML =
                                                getTorrentResultTableHtml(torrents);
                                        });
                                    },
                                        5000);
                                    break;

                            }
                        });
                    });

                });

            });
        }

        function drawDownloadChart(view, Chart) {

            var ctx = view.querySelector('#downloadSpeedChart').getContext("2d");

            return new Chart(ctx,
                {
                    type: 'line',
                    data: {
                        labels: [new Date().getSeconds()],
                        datasets: [{
                            label: 'Download Speed',
                            borderColor: "#4584b5",
                            fill: false,
                            data: [0]
                        }]
                    },
                    options: {
                        title: {
                            display: true
                        },
                        responsive: true,
                        maintainAspectRatio: false
                    }
                });


        }

        return function (view) {
            view.addEventListener('viewshow',
                () => { 

                    loading.show();

                    loadConfig(view);

                    view.querySelector('#openTorrentDialog').addEventListener('click',
                        () => {
                            loading.show();
                            clearInterval(torrentMonitor);
                            openSettingsDialog(view);
                            loading.hide();
                        });

                    view.querySelector('#openAddTorrentDialog').addEventListener('click',
                        () => {
                            openAddTorrentDialog();
                        });

                    view.querySelector('#enableRealTimeMonitoring').addEventListener('change',
                        () => {

                            realTimeMonitor = view.querySelector('#enableRealTimeMonitoring').checked;
                            ApiClient.getPluginConfiguration(pluginId).then(
                                (config) => {
                                    config.EnableRealtimeMonitoring = view.querySelector('#enableRealTimeMonitoring').checked;

                                    ApiClient.updatePluginConfiguration(pluginId, config).then(() => {
                                        loadPageData(view, config);
                                    });
                                });
                        });

                });

            view.addEventListener('viewhide', () => {
                realTimeMonitor = false;
            });

            view.addEventListener('viewdestroy', () => {
                realTimeMonitor = false;
            });

        }
    });