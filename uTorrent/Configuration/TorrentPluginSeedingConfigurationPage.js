define(["loading", "dialogHelper",  "mainTabsManager","formDialogStyle"],
    function (loading, dialogHelper, mainTabsManager) {

        var pluginId = "b1390c15-5b4f-4038-bb58-b71b9ef4211b";
        var loaded;
        var providerImages = [];

        var pagination = {
            StartIndex      : 0,
            Limit           : 25,
            TotalRecordCount: 0
        }

        //Backend Sorting Enum:
        //NAME_ASCENDING  = 0,
        //NAME_DESCENDING = 1,
        //DATE_ASCENDING  = 2,
        //DATE_DESCENDING = 3
        //CONTENT_MOVIE = 4
        //CONTENT_SERIES = 5
        
        var directionSvg = {
            ascending : "M7,10L12,15L17,10H7Z",
            descending: "M7,15L12,10L17,15H7Z"
        }

        

        var sortBy = {
            name_ascending : 0, 
            name_descending: 1,
            date_ascending : 2,
            date_descending: 3,
            content_movie  : 4,
            content_series: 5
        }

        var currentSort = sortBy.date_descending;
        
        function getTabs() {
            return [
                {
                    href: Dashboard.getConfigurationPageUrl('TorrentPluginDownloadingConfigurationPage'),
                    name: "Downloading"
                },
                {
                    href: Dashboard.getConfigurationPageUrl('TorrentPluginSeedingConfigurationPage'),
                    name: "Seeding"
                },
                {
                    href: Dashboard.getConfigurationPageUrl('TorrentPluginSettingsConfigurationPage'),
                    name: "Settings"
                },
                {
                    href: Dashboard.getConfigurationPageUrl('TorrentPluginUploadConfigurationPage'),
                    name: "Upload"
                },
                {
                    href: Dashboard.getConfigurationPageUrl('TorrentPluginPostProcessingConfigurationPage'),
                    name: "Post Processing"
                },
                //{
                //    href: Dashboard.getConfigurationPageUrl('TorrentPluginRssConfigurationPage'),
                //    name: "Rss"
                //}
            ];
        }

        function getPagingHtml() {

            var html = '';
            html += '<div class="listPaging">';

            const recordEnd = pagination.StartIndex + pagination.Limit > pagination.TotalRecordCount ? pagination.TotalRecordCount : pagination.StartIndex + pagination.Limit;
            html += '<span style="vertical-align:middle;">';
            html += pagination.StartIndex + 1 + '-' + recordEnd + ' of ' + pagination.TotalRecordCount;

            html += '</span>';

            html += '<div style="display:inline-block;">';

            html += '<button is="paper-icon-button-light" class="btnPreviousPage autoSize" ' + (pagination.StartIndex ? '' : 'disabled') + '><i class="md-icon">&#xE5C4;</i></button>';
            html += '<button is="paper-icon-button-light" class="btnNextPage autoSize" ' + (pagination.StartIndex + pagination.Limit >= pagination.TotalRecordCount ? 'disabled' : '') + '><i class="md-icon">&#xE5C8;</i></button>';

            html += '</div>';


            html += '</div>';

            return html;
        }

        function extractedIcon(extracted) {
            switch (extracted) {
                case true: return "M12 2C6.5 2 2 6.5 2 12S6.5 22 12 22 22 17.5 22 12 17.5 2 12 2M10 17L5 12L6.41 10.59L10 14.17L17.59 6.58L19 8L10 17Z";
                case false: return "M13,2V4C17.39,4.54 20.5,8.53 19.96,12.92C19.5,16.56 16.64,19.43 13,19.88V21.88C18.5,21.28 22.45,16.34 21.85,10.85C21.33,6.19 17.66,2.5 13,2M11,2C9.04,2.18 7.19,2.95 5.67,4.2L7.1,5.74C8.22,4.84 9.57,4.26 11,4.06V2.06M4.26,5.67C3,7.19 2.24,9.04 2.05,11H4.05C4.24,9.58 4.8,8.23 5.69,7.1L4.26,5.67M2.06,13C2.26,14.96 3.03,16.81 4.27,18.33L5.69,16.9C4.81,15.77 4.24,14.42 4.06,13H2.06M7.06,18.37L5.67,19.74C7.18,21 9.04,21.79 11,22V20C9.58,19.82 8.23,19.25 7.1,18.37H7.06M13,13V7H11V13H13M13,17V15H11V17H13Z";
            }
        }

        function getResolutionIcon(resolution) {
            switch (resolution) {
                case "1080p":
                    return '<svg x="0px" y="0px" viewBox="0 0 290.262 290.262" style="enable-background:new 0 0 290.262 290.262;width: 36px;background-color: black;" fill="goldenrod" xml:space="preserve"><g id="_x34_2-_1080_Full_HD"><path d="M278.743,29.29H11.519C5.157,29.29,0,34.447,0,40.809v128.645v11.355v68.645c0,6.361,5.157,11.519,11.519,11.519h267.225   c6.361,0,11.519-5.157,11.519-11.519v-68.645v-11.355V40.809C290.262,34.447,285.104,29.29,278.743,29.29z M56.563,185.959H33.751   v15.375H54.19v4.813H33.751v18.748h-4.996v-43.748h27.809V185.959z M99.69,206.895c0,11.375-6.875,18.252-18.313,18.252   c-11.5,0-18.436-6.877-18.436-18.252v-25.748h5v25.748c0,8.5,5.122,13.439,13.436,13.439c8.313,0,13.313-4.939,13.313-13.439   v-25.748h5V206.895z M136.13,224.895h-24.188v-43.748h5v39.002h19.188V224.895z M168.444,224.895h-24.187v-43.748h4.998v39.002   h19.189V224.895z M214.693,224.895h-11.126v-16.998h-18.121v16.998h-11.127v-43.748h11.127v18h18.121v-18h11.126V224.895z    M241.822,224.895h-18.376v-25.201h11.125v16.33h7.939c6.811,0,11.688-5.254,11.688-12.939c0-7.754-5.126-13.063-12.189-13.063   h-18.563v-8.875h18.813c13.75,0,23.248,8.875,23.248,21.873C265.507,215.957,255.882,224.895,241.822,224.895z M267.225,157.935   H23.037V52.327h244.188V157.935z"/><polygon points="53.415,128.666 66.592,128.666 66.592,76.775 43.866,76.775 43.866,87.363 53.415,87.363  "/><path d="M99.901,129.037c14.656,0,22.873-9.404,22.873-26.354c0-16.877-8.217-26.279-22.873-26.279   c-14.805,0-23.021,9.402-23.021,26.279C76.88,119.633,85.097,129.037,99.901,129.037z M99.901,86.029c6.514,0,9.4,4.813,9.4,16.654   s-2.887,16.729-9.4,16.729c-6.664,0-9.475-4.887-9.475-16.729S93.237,86.029,99.901,86.029z"/> <path d="M128.401,114.232c0,9.178,8.29,15.025,21.246,15.025c12.951,0,21.243-5.922,21.243-15.25   c0-5.771-3.552-10.732-9.253-13.102c4.072-2.221,6.514-6.217,6.514-10.734c0-8.512-7.18-13.914-18.58-13.914   c-11.25,0-18.505,5.258-18.505,13.549c0,4.512,2.814,8.656,7.18,11.1C132.251,103.275,128.401,108.307,128.401,114.232z    M149.647,84.918c4.811,0,7.475,2.148,7.475,5.994c0,3.703-2.664,5.777-7.475,5.777c-4.813,0-7.477-2.074-7.477-5.777   C142.17,87.066,144.835,84.918,149.647,84.918z M149.647,106.164c5.697,0,8.881,2.441,8.881,6.736c0,4.441-3.184,6.811-8.881,6.811   c-5.701,0-8.809-2.445-8.809-6.811C140.839,108.605,143.946,106.164,149.647,106.164z"/> <path d="M199.466,129.037c14.655,0,22.872-9.404,22.872-26.354c0-16.877-8.217-26.279-22.872-26.279   c-14.805,0-23.023,9.402-23.023,26.279C176.443,119.633,184.661,129.037,199.466,129.037z M199.466,86.029   c6.514,0,9.398,4.813,9.398,16.654s-2.885,16.729-9.398,16.729c-6.662,0-9.475-4.887-9.475-16.729S192.804,86.029,199.466,86.029z"/><path d="M234.948,121.119h4.865c6.857,0,10.803-3.641,10.803-9.924c0-5.973-3.945-9.346-10.803-9.346h-11.682v26.816h6.816V121.119   z M234.948,107.213h4.521c2.987,0,4.712,1.414,4.712,4.217c0,2.832-1.725,4.326-4.712,4.326h-4.521V107.213z"/></g></svg>';
                case "720p":
                    return "";
                case "2160p":
                    return '<svg x="0px" y="0px" viewBox="0 0 290.262 290.262" style="enable-background:new 0 0 290.262 290.262;width: 36px;background-color: black;" fill="goldenrod" xml:space="preserve"><g id="_x34_3-4k_Full_HD"><path d="M278.743,29.29H11.519C5.157,29.29,0,34.447,0,40.809v128.645v11.355v68.645c0,6.361,5.157,11.519,11.519,11.519h267.225   c6.361,0,11.519-5.157,11.519-11.519v-68.645v-11.355V40.809C290.262,34.447,285.104,29.29,278.743,29.29z M56.563,185.959H33.751   v15.375H54.19v4.813H33.751v18.748h-4.996v-43.748h27.809V185.959z M99.69,206.895c0,11.375-6.875,18.252-18.313,18.252   c-11.5,0-18.436-6.877-18.436-18.252v-25.748h5v25.748c0,8.5,5.122,13.439,13.436,13.439c8.313,0,13.313-4.939,13.313-13.439   v-25.748h5V206.895z M136.13,224.895h-24.188v-43.748h5v39.002h19.188V224.895z M168.444,224.895h-24.188v-43.748h4.998v39.002   h19.189V224.895z M214.693,224.895h-11.126v-16.998h-18.121v16.998h-11.127v-43.748h11.127v18h18.121v-18h11.126V224.895z    M241.822,224.895h-18.376v-25.201h11.125v16.33h7.938c6.812,0,11.688-5.254,11.688-12.939c0-7.754-5.126-13.063-12.188-13.063   h-18.563v-8.875h18.813c13.75,0,23.248,8.875,23.248,21.873C265.507,215.957,255.882,224.895,241.822,224.895z M267.225,157.935   H23.037V52.327h244.188V157.935z"/><polygon points="106.752,143.336 125.882,143.336 125.882,125.48 136.895,125.48 136.895,109.83 125.882,109.83 125.882,92.439    106.752,92.439 106.752,109.83 89.594,109.83 113.708,62.18 93.883,62.18 68.261,111.686 68.261,125.48 106.752,125.48  "/><polygon points="167.275,123.395 178.057,111.453 199.505,143.336 224.199,143.336 192.085,95.801 222.229,62.18 198.346,62.18    167.275,97.309 167.275,62.18 146.638,62.18 146.638,143.336 167.275,143.336  "/></g></svg>';

            }
        }

        function getResultContentTypeIcon(type) {
            switch (type) {
            case "UNKNOWN": return '';
            case "MOVIE":
                return '<path fill="var(--theme-primary-color)" d="M14.75 5.46L12 1.93L13.97 1.54L16.71 5.07L14.75 5.46M21.62 4.1L20.84 .18L16.91 .96L19.65 4.5L21.62 4.1M11.81 6.05L9.07 2.5L7.1 2.91L9.85 6.44L11.81 6.05M2 8V18C2 19.11 2.9 20 4 20H20C21.11 20 22 19.11 22 18V8H2M4.16 3.5L3.18 3.69C2.1 3.91 1.4 4.96 1.61 6.04L2 8L6.9 7.03L4.16 3.5M11 24H13V22H11V24M7 24H9V22H7V24M15 24H17V22H15V24Z" />';
            case "SERIES":
                return '<path fill="var(--theme-primary-color)" d="M8.16,3L6.75,4.41L9.34,7H4C2.89,7 2,7.89 2,9V19C2,20.11 2.89,21 4,21H20C21.11,21 22,20.11 22,19V9C22,7.89 21.11,7 20,7H14.66L17.25,4.41L15.84,3L12,6.84L8.16,3M4,9H17V19H4V9M19.5,9A1,1 0 0,1 20.5,10A1,1 0 0,1 19.5,11A1,1 0 0,1 18.5,10A1,1 0 0,1 19.5,9M19.5,12A1,1 0 0,1 20.5,13A1,1 0 0,1 19.5,14A1,1 0 0,1 18.5,13A1,1 0 0,1 19.5,12Z" />'; 
            case "SONG":
                return '<path fill="var(--theme-primary-color)" d="M21,3V15.5A3.5,3.5 0 0,1 17.5,19A3.5,3.5 0 0,1 14,15.5A3.5,3.5 0 0,1 17.5,12C18.04,12 18.55,12.12 19,12.34V6.47L9,8.6V17.5A3.5,3.5 0 0,1 5.5,21A3.5,3.5 0 0,1 2,17.5A3.5,3.5 0 0,1 5.5,14C6.04,14 6.55,14.12 7,14.34V6L21,3Z" />';
            }
        }

        async function getImage(name, type, year = 0) {

            const options = {
                SearchInfo: {
                    Name: name
                }
            };
            if (year > 0) {
                options.SearchInfo.Year = year;
            }

            const url = ApiClient._serverAddress + '/emby/' + 'Items/RemoteSearch/' + type + '?api_key=' + ApiClient._serverInfo.AccessToken;
            const rawResponse = await fetch(url, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(options)
            });
            const content = await rawResponse.json();
            
            return content[0];
        }

        function injectImages(view, torrents) {
            try {
                const rows = view.querySelectorAll('tr');
                rows.forEach(async row => {
                    var img = row.querySelector('img');
                    if (img) {
                        var hash = img.id.split('img_')[1];
                        var torrent = torrents.filter(t => t.Hash == hash)[0];
                        if (torrent) {
                            if (!providerImages.filter(p => p.Name.toLowerCase() == torrent.MediaInfo.SortName.toLowerCase())[0]) {
                                const providerData = await getImage(torrent.MediaInfo.SortName, torrent.MediaInfo.MediaType.toLowerCase(), torrent.MediaInfo.Year);
                                providerImages.push({
                                    Name: torrent.MediaInfo.SortName,
                                    ImageUrl: providerData.ImageUrl
                                });
                            }
                            //place the image in the appropriate row.
                            img.src = providerImages.filter(p => p.Name.toLowerCase() == torrent.MediaInfo.SortName.toLowerCase())[0].ImageUrl;
                        }
                    }
                });
            } catch (err) {

            }
        }

        function renderTableRowHtml(torrent, updateRow) {
            var html = '';
            html += '<tr class="detailTableBodyRow detailTableBodyRow-shaded" id="' + torrent.Hash + '">';
            html += '<td class="detailTableBodyCell" data-title="Status">';
            html += '<svg style="width:24px;height:24px" viewBox="0 0 24 24">';

            //Oh we're doing this.
            //Status can be "started" when seeding... ugh!
            //If the progress is 1000 we are seeding. We have to change the Status. and recall a method to find its actual status properly.
            //This can not be done on the backend easily. We must figure it out here.
            var findStatus = function (torrent, updateRow) {
                switch (torrent.Status) {
                    //play icon
                    case 'started':
                        {
                            if (torrent.Progress != 1000) { //definitely download this file.
                                html += '<path fill="var(--theme-primary-color)" d="M8,5.14V19.14L19,12.14L8,5.14Z" />';
                                break;
                            } else { //definitely not download this file. Seeding it! Set it to the queue icon. just shut up about it.
                                torrent.Status = "queued";
                                findStatus(torrent, updateRow);
                                break;
                            }
                        }
                    //stop icon
                    case 'stopped':
                        html += '<path fill="currentColor" d="M18,18H6V6H18V18Z" />';
                        break;
                    //pause icon
                    case 'paused':
                        html +=
                            '<path fill="currentColor" d="M22 12C22 6.46 17.54 2 12 2C10.83 2 9.7 2.19 8.62 2.56L9.32 4.5C10.17 4.16 11.06 3.97 12 3.97C16.41 3.97 20.03 7.59 20.03 12C20.03 16.41 16.41 20.03 12 20.03C7.59 20.03 3.97 16.41 3.97 12C3.97 11.06 4.16 10.12 4.5 9.28L2.56 8.62C2.19 9.7 2 10.83 2 12C2 17.54 6.46 22 12 22C17.54 22 22 17.54 22 12M5.47 7C4.68 7 3.97 6.32 3.97 5.47C3.97 4.68 4.68 3.97 5.47 3.97C6.32 3.97 7 4.68 7 5.47C7 6.32 6.32 7 5.47 7M9 9H11V15H9M13 9H15V15H13" />';
                        break;
                    //sync icon
                    case 're-check':
                        html +=
                            '<path fill="goldenrod" d="M12,18A6,6 0 0,1 6,12C6,11 6.25,10.03 6.7,9.2L5.24,7.74C4.46,8.97 4,10.43 4,12A8,8 0 0,0 12,20V23L16,19L12,15M12,4V1L8,5L12,9V6A6,6 0 0,1 18,12C18,13 17.75,13.97 17.3,14.8L18.76,16.26C19.54,15.03 20,13.57 20,12A8,8 0 0,0 12,4Z" />';
                        break;
                    case "seeding [F]":
                    case "queued":
                        {
                            if (torrent.Progress == 1000) {
                                //seeding
                                //network computer connected icon 
                                html += ' <path fill="' +
                                    (updateRow ? 'var(--theme-primary-color)' : 'currentColor') +
                                    '" d="M4,1C2.89,1 2,1.89 2,3V7C2,8.11 2.89,9 4,9H1V11H13V9H10C11.11,9 12,8.11 12,7V3C12,1.89 11.11,1 10,1H4M4,3H10V7H4V3M3,12V14H5V12H3M14,13C12.89,13 12,13.89 12,15V19C12,20.11 12.89,21 14,21H11V23H23V21H20C21.11,21 22,20.11 22,19V15C22,13.89 21.11,13 20,13H14M3,15V17H5V15H3M14,15H20V19H14V15M3,18V20H5V18H3M6,18V20H8V18H6M9,18V20H11V18H9Z" />';
                            } else {
                                //waiting to start - clock icon
                                html +=
                                    '<path fill="currentColor" d="M12 20C16.4 20 20 16.4 20 12S16.4 4 12 4 4 7.6 4 12 7.6 20 12 20M12 2C17.5 2 22 6.5 22 12S17.5 22 12 22C6.5 22 2 17.5 2 12C2 6.5 6.5 2 12 2M17 13.9L16.3 15.2L11 12.3V7H12.5V11.4L17 13.9Z" />';
                            }
                            break;
                        }
                }
            }

            findStatus(torrent, updateRow);

            html += '</svg>';
            html += '</td>';
            //Banner/Name - Only show images for tv shows. The poster image doesn't fit the style in the layout with all the banners.
            html += '<td data-title="Name" class="detailTableBodyCell fileCell">';

            //if (torrent.MediaInfo.MediaType && torrent.MediaInfo.MediaType == "SERIES") {
            //html += '<div style="max-height:7em; overflow:hidden">';
            //html += '<img style="height:100%" id="img_' + torrent.Hash + '"/>';
            //html += '</div>';
            //} else {
            html += torrent.MediaInfo ? torrent.MediaInfo.SortName : "";
            //}
           
            html += '</td>';
            //Content Type  Icon
            html += '<td data-title="Content" class="detailTableBodyCell fileCell">';
            html += '<svg style="height:24px" viewBox="0 0 24 24">';
            html += torrent.MediaInfo ? getResultContentTypeIcon(torrent.MediaInfo.MediaType) : "";
            html += '</svg>';
            html += '</td>';
            html += '<td data-title="Resolution" class="detailTableBodyCell fileCell">' + (torrent.MediaInfo ? torrent.MediaInfo.Resolution : "") + '</td>';
            //FileName
            html += '<td data-title="File Name" class="detailTableBodyCell fileCell">' + torrent.Name + '</td>';
            //Extension
            //html += '<td data-title="Size" class="detailTableBodyCell fileCell">' + torrent.Extension + '</td>';
            //Size
            html += '<td data-title="Size" class="detailTableBodyCell fileCell">' + torrent.Size + '</td>';
            //Speed
            html += '<td data-title="Speed" class="detailTableBodyCell fileCell">' + torrent.DownloadSpeedFriendly + '/s</td>';
            //Completed
            html += '<td data-title="Complete" class="detailTableBodyCell fileCell"><span>' + (torrent.Progress / 10) + '%</span></td>';
            //Extracted
            html += '<td data-title="Extracted" class="detailTableBodyCell fileCell">';
            html += '<svg style="width:24px;height:24px" viewBox="0 0 24 24">';
            html += '<path fill="var(--theme-primary-color)" d="';
            html += extractedIcon(torrent.Extracted);
            html += '" />';
            html += '</svg>';
            html += '</td>';
            //Ratio
            html += '<td data-title="Ratio" class="detailTableBodyCell fileCell">';
            html += '<svg style="width:24px;height:24px" viewBox="0 0 24 24">';
            if (torrent.Ratio / 100 > 1) {
                html += '<path fill="var(--theme-primary-color)" d="M19,19H5V8H19M19,3H18V1H16V3H8V1H6V3H5C3.89,3 3,3.9 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5A2,2 0 0,0 19,3M16.53,11.06L15.47,10L10.59,14.88L8.47,12.76L7.41,13.82L10.59,17L16.53,11.06Z" />';
            } else {
                html += '<path fill="goldenrod" d="M15,13H16.5V15.82L18.94,17.23L18.19,18.53L15,16.69V13M19,8H5V19H9.67C9.24,18.09 9,17.07 9,16A7,7 0 0,1 16,9C17.07,9 18.09,9.24 19,9.67V8M5,21C3.89,21 3,20.1 3,19V5C3,3.89 3.89,3 5,3H6V1H8V3H16V1H18V3H19A2,2 0 0,1 21,5V11.1C22.24,12.36 23,14.09 23,16A7,7 0 0,1 16,23C14.09,23 12.36,22.24 11.1,21H5M16,11.15A4.85,4.85 0 0,0 11.15,16C11.15,18.68 13.32,20.85 16,20.85A4.85,4.85 0 0,0 20.85,16C20.85,13.32 18.68,11.15 16,11.15Z" />';
            }
            html += '</svg>';
            html += '</td>';
            //Date Added
            html += '<td data-title="Date Added" class="detailTableBodyCell fileCell">' + torrent.AddedDate + '</td>';
            //Spacer
            html += '<td class="detailTableBodyCell organizerButtonCell" style="whitespace:no-wrap;"></td>';

            html += '</tr>';
            return html;
        }

        function getTorrentResultTableHtml(torrents, view) {
            var html = '';
            torrents.forEach(async torrent => {
                html += renderTableRowHtml(torrent, false);
            });
            return html;
        }
        
        
       
        async function getTorrentUpdates() {
            const result = await ApiClient.getJSON(ApiClient.getUrl("GetTorrentDataUpdate"));
            return result;
        }
        
        function removeTorrent(hash, config) {
            return new Promise((resolve, reject) => {
                ApiClient.getJSON(ApiClient.getUrl("RemoveTorrent?Id=" + hash)).then((result) => {
                    resolve(result);
                });
            });
        }
        
        async function getUTorrentData() {
            const result = await ApiClient.getJSON(ApiClient.getUrl("GetTorrentData?StartIndex=" + pagination.StartIndex + "&Limit=" + pagination.Limit + '&SortBy=' + currentSort + ''));
            return result;
        }
       

        function remoteControlTorrent(remoteCommand, id, config) {
            return new Promise((resolve, reject) => {
                ApiClient.getJSON(ApiClient.getUrl(
                    remoteCommand +
                    "?IpAddress=" +
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
        }

        async function updatePageData(view, config, results) {

            const pagingContainer = view.querySelector('.pagingContainer');
            pagination.TotalRecordCount = results.TotalRecordCount;
            pagingContainer.innerHTML = '';
            pagingContainer.innerHTML += getPagingHtml();
            
            view.querySelector('.btnPreviousPage').addEventListener('click',
                async (btn) => {
                    btn.preventDefault();
                    loading.show();
                    pagingContainer.innerHTML = '';
                    pagination.StartIndex -= pagination.Limit;
                    results = await getUTorrentData();
                    await updatePageData(view, config, results);
                });

            view.querySelector('.btnNextPage').addEventListener('click',
                async (btn) => {
                    btn.preventDefault();
                    loading.show();
                    pagingContainer.innerHTML = '';
                    pagination.StartIndex += pagination.Limit;
                    results = await getUTorrentData();
                    await updatePageData(view, config, results);
                });

           
            view.querySelector('.torrentResultBody').innerHTML = getTorrentResultTableHtml(results.torrents, view);

            //injectImages(view, results.torrents);

            Dashboard.hideLoadingMsg();

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

           

        }

        
        function monitorTorrents(view) {
            setTimeout(async () => {
                if (!loaded) return;

                var result;
                try {
                    result = await getTorrentUpdates();
                } catch (err) {
                    console.log(err);
                    return;
                }

                var table = view.querySelector('.torrentResultBody');

                //reset svg colors to default
                var paths = table.querySelectorAll('td[data-title="Status"] svg path');
                for (let i = 0; i <= paths.length-1; i++) {
                    paths[i].setAttribute('fill', "currentColor");
                }


                for(let i = 0; i <= result.torrents.length-1; i++) {
                    var row;

                    try {
                        
                        row = document.getElementById(result.torrents[i].Hash);

                    } catch(err) {
                        console.log(err);
                    }
                    if (row) {
                        row.innerHTML = renderTableRowHtml(result.torrents[i], true);
                        //injectImages(view, result.torrents);
                    }
                }
                monitorTorrents(view);

            }, 5000);
        }

        return function (view) {
            view.addEventListener('viewshow',
                async () => {

                    loading.show();

                    mainTabsManager.setTabs(this, 1, getTabs);

                    var config = await ApiClient.getPluginConfiguration(pluginId);

                    if (!config.ip && !config.port) {
                        loading.hide();
                        return;
                    }

                    var results;
                    
                    try {

                        results = await getUTorrentData();

                    } catch (err) {

                    }
                   
                    if(results) await updatePageData(view, config, results);
                    loaded = true;

                    monitorTorrents(view);

                    //Sort by name
                    view.querySelector('.name_sort').addEventListener('click', async (e) => {
                        e.preventDefault();
                        if (currentSort === sortBy.name_ascending) {
                            currentSort = sortBy.name_descending;
                            results = await getUTorrentData();
                            await updatePageData(view, config, results);

                           
                            return;
                        }
                 
                        currentSort = sortBy.name_ascending;
                        results = await getUTorrentData();
                        await updatePageData(view, config, results);
                        
                    });

                    //Sort by date added - Default
                    view.querySelector('.date_sort').addEventListener('click', async (e) => {
                        e.preventDefault();
                        if (currentSort === sortBy.date_ascending) {
                            currentSort = sortBy.date_descending;
                            results = await getUTorrentData();
                            await updatePageData(view, config, results);
                           
                            return;
                        }
                        currentSort = sortBy.date_ascending;
                        results = await getUTorrentData();
                        await updatePageData(view, config, results);
                        
                    });

                    //SOrt by content type (movie/tv_show)
                    view.querySelector('.content_sort').addEventListener('click', async (e) => {
                        e.preventDefault();
                        if (currentSort === sortBy.content_movie) {
                            currentSort = sortBy.content_series;
                            results = await getUTorrentData();
                            await updatePageData(view, config, results);
                            
                            return;
                        }

                        currentSort = sortBy.content_movie;
                        results = await getUTorrentData();
                        await updatePageData(view, config, results);
                       
                    });

                });
            view.addEventListener('viewhide',
                () => {
                    loaded = false;
                });
        }
        
    });