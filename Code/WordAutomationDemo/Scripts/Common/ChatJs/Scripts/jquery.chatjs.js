/**
* ChatJS 1.0 - MIT License
* www.chatjs.net
* 
* Copyright (c) 2013, André Pena
* All rights reserved.
*
**/
// CHAT CONTAINER
(function ($) {
    function ChatContainer(options) {       
        this.defaults = {
            objectType: null,
            objectName: null,
            title: null,
            canClose: true,
            showTextBox: true,
            initialToggleState: "maximized",
            onCreated: function (chatContainer) { },
            onClose: function (chatContainer) { },            
            onToggleStateChanged: function (currentState) { }
        };

        //Extending options:
        this.opts = $.extend({}, this.defaults, options);

        //Privates:
        this.$el = null;
        this.$window = null;
        this.$windowTitle = null;
        this.$windowContent = null;
        this.$windowInnerContent = null;
        this.$textBox = null;
        this.$linkButton = null;
        this.$OfflineMsg = null;
    }

    // Separate functionality from object creation
    ChatContainer.prototype = {
        init: function () {
            var _this = this;
            _this.$window = $("<div/>").addClass("chat-window").appendTo($("body"));
            _this.$windowTitle = $("<div/>").addClass("chat-window-title").appendTo(_this.$window);
            if (_this.opts.title == "Messaging") {
                _this.$XYZ = $("<span onclick='OpenInstantChatBox();' title='Instant Message'>&nbsp;</span>").addClass("InstantMsg").appendTo(_this.$window);
            }
            if (_this.opts.canClose) {
                if (_this.opts.title != "Messaging") {
                    var $closeButton = $("<div/>").addClass("close").appendTo(_this.$windowTitle);
                    $closeButton.click(function (e) {
                        e.stopPropagation();
                        for (var i = 0; i < $._chatContainers.length; i++) {
                            if ($._chatContainers[i] == _this) {
                                $._chatContainers.splice(i, 1);
                                break;
                            }
                        }
                        _this.$window.remove(); // remove();
                        _this.opts.onClose(_this);
                    });
                }
            }
            $("<div/>").addClass("text").text(_this.opts.title).appendTo(_this.$windowTitle);
            _this.$windowContent = $("<div/>").addClass("chat-window-content").appendTo(_this.$window);
            _this.$windowContent.hide();
            if (_this.opts.title != "Messaging") {
                var $msgClear = $("<div/>").addClass("chat-window-text-box-wrapper").appendTo(_this.$windowContent);
                _this.$linkButton = $("<a />").addClass("chat-window-Clear-History").appendTo($msgClear);
                _this.$linkButton.text('Clear History');
                var $msgClear = $("<div/>").addClass("chat-window-text-box-wrapper").appendTo(_this.$windowContent);
                _this.$OfflineMsg = $("<div />").addClass("chat-window-Offline-Message").appendTo($msgClear);
                _this.$OfflineMsg.text("#UserName# isn't on ready portal right now. #UserName# will see your messages later, when online.");
                _this.$windowContent.show();
            }
            _this.$windowInnerContent = $("<div/>").addClass("chat-window-inner-content").appendTo(_this.$windowContent);
            if (_this.opts.showTextBox) {
                var $windowTextBoxWrapper = $("<div/>").addClass("chat-window-text-box-wrapper").appendTo(_this.$windowContent);
                _this.$textBox = $("<textarea />").attr("rows", "1").css('min-height', '26px').addClass("chat-window-text-box").addClass("form-control").appendTo($windowTextBoxWrapper);
                _this.$textBox.autosize();
            }
            _this.$windowTitle.click(function () {
                _this.$windowContent.toggle();
                if (_this.$windowContent.is(":visible") && _this.opts.showTextBox)
                    _this.$textBox.focus();
                _this.opts.onToggleStateChanged(_this.$windowContent.is(":visible") ? "maximized" : "minimized");
            });
            if (!$._chatContainers)
                $._chatContainers = new Array();
            $._chatContainers.push(_this);
            $.organizeChatContainers();
            _this.opts.onCreated(_this);
        },
        getContent: function () {
            var _this = this;
            return _this.$windowInnerContent;
        },
        setTitle: function (title) {
            var _this = this;
            $("div[class=text]", _this.$windowTitle).text(title);
        },
        setVisible: function (visible) {
            var _this = this;
            if (visible)
                _this.$window.show();
            else
                _this.$window.hide();
        },
        getToggleState: function () {
            var _this = this;
            return _this.$windowContent.is(":visible") ? "maximized" : "minimized";
        },
        setToggleState: function (state) {
            var _this = this;
            if (state == "minimized")
                _this.$windowContent.hide();
            else if (state == "maximized")
                _this.$windowContent.show();
        }
    };
    $.chatContainer = function (options) {
        var chatContainer = new ChatContainer(options);
        chatContainer.init();
        return chatContainer;
    };
    $.organizeChatContainers = function () {
        var rightOffset = 10;
        var deltaOffset = 10;
        for (var i = 0; i < $._chatContainers.length; i++) {
            $._chatContainers[i].$window.css("right", rightOffset);
            rightOffset += $._chatContainers[i].$window.outerWidth() + deltaOffset;
        }
    };
})(jQuery);
function parseISO8601(dateStringInRange) {
    var isoExp = /^\s*(\d{4})-(\d\d)-(\d\d)\s*$/,
        date = new Date(NaN), month,
        parts = isoExp.exec(dateStringInRange);
    if (parts) {
        month = +parts[2];
        date.setFullYear(parts[1], month - 1, parts[3]);
        if (month != date.getMonth() + 1) {
            date.setTime(NaN);
        }
    }
    return date;
}
(function ($) {
    function ChatWindow(options) {
        this.defaults = {
            chat: null,
            myUser: null,
            otherUser: null,
            typingText: null,
            initialToggleState: "maximized",
            initialFocusState: "focused",
            userIsOnline: false,
            adapter: null,
            onReady: function () { },
            onClose: function (container) { },
            onToggleStateChanged: function (currentState) { }
        };
        this.opts = $.extend({}, this.defaults, options);
        this.$el = null;
        this.chatContainer = null;
        this.addMessage = function (message, firstlastName, clientGuid) {
            var _this = this;
            _this.chatContainer.setToggleState("maximized");
            if (message.UserFromId != this.opts.myUser.Id) {
                _this.removeTypingSignal();
            }
            function linkify($element) {
                var inputText = $element.html();
                var replacedText, replacePattern1, replacePattern2, replacePattern3;
                replacePattern1 = /(\b(https?|ftp):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/gim;
                replacedText = inputText.replace(replacePattern1, '<a href="$1" target="_blank">$1</a>');
                replacePattern2 = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
                replacedText = replacedText.replace(replacePattern2, '$1<a href="http://$2" target="_blank">$2</a>');
                replacePattern3 = /(\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,6})/gim;
                replacedText = replacedText.replace(replacePattern3, '<a href="mailto:$1">$1</a>');
                return $element.html(replacedText);
            }
            function emotify($element) {
                var inputText = $element.html();
                var replacedText = inputText;
                var emoticons = [
                    { pattern: ":-\)", cssClass: "happy" },
                    { pattern: ":\)", cssClass: "happy" },
                    { pattern: "=\)", cssClass: "happy" },
                    { pattern: ":-D", cssClass: "very-happy" },
                    { pattern: ":D", cssClass: "very-happy" },
                    { pattern: "=D", cssClass: "very-happy" },
                    { pattern: ":-\(", cssClass: "sad" },
                    { pattern: ":\(", cssClass: "sad" },
                    { pattern: "=\(", cssClass: "sad" },
                    { pattern: ":-\|", cssClass: "wary" },
                    { pattern: ":\|", cssClass: "wary" },
                    { pattern: "=\|", cssClass: "wary" },
                    { pattern: ":-O", cssClass: "astonished" },
                    { pattern: ":O", cssClass: "astonished" },
                    { pattern: "=O", cssClass: "astonished" },
                    { pattern: ":-P", cssClass: "tongue" },
                    { pattern: ":P", cssClass: "tongue" },
                    { pattern: "=P", cssClass: "tongue" }
                ];
                for (var i = 0; i < emoticons.length; i++) {
                    replacedText = replacedText.replace(emoticons[i].pattern, "<span class='" + emoticons[i].cssClass + "'></span>");
                }
                return $element.html(replacedText);
            }
            if (message.ClientGuid && $("p[data-val-client-guid='" + message.ClientGuid + "']").length) {
                // in this case, this message is comming from the server AND the current user POSTED the message.
                // so he/she already has this message in the list. We DO NOT need to add the message.
                $("p[data-val-client-guid='" + message.ClientGuid + "']").removeClass("temp-message").removeAttr("data-val-client-guid");
            } else {
                var $messageP = $("<p/>").text(message.Message);
                if (clientGuid)
                    linkify($messageP);
                emotify($messageP);
                var $lastMessage = $("div.chat-message:last", _this.chatContainer.$windowInnerContent);                
                var $chatMessage = $("<div/>").addClass("chat-message").attr("data-val-user-from", message.UserFromId);
                $chatMessage.appendTo(_this.chatContainer.$windowInnerContent);
                var dateValuesRelace1 = message.MessageSentDateTime.replace('T', '-');
                var dateValuesRelace2 = dateValuesRelace1.replace(/:/g, '-');
                var dateValues = dateValuesRelace2.split('-');
                var year_part = dateValues[0];
                var month_part = dateValues[1];
                var day_part = dateValues[2];
                var hour_part = dateValues[3];
                var min_part = dateValues[4];
                var sec_part = dateValues[5].substring(0, 6);  // getting second part with three decimal (15.233)                   
                var month_names = new Array();
                month_names[month_names.length] = "Jan";
                month_names[month_names.length] = "Feb";
                month_names[month_names.length] = "Mar";
                month_names[month_names.length] = "Apr";
                month_names[month_names.length] = "May";
                month_names[month_names.length] = "Jun";
                month_names[month_names.length] = "Jul";
                month_names[month_names.length] = "Aug";
                month_names[month_names.length] = "Sep";
                month_names[month_names.length] = "Oct";
                month_names[month_names.length] = "Nov";
                month_names[month_names.length] = "Dec";
                var msgDated = new Date(year_part, month_part - 1, day_part, hour_part, min_part, sec_part);
                var day = msgDated.getDate();
                var month = month_names[msgDated.getMonth()];
                var year = msgDated.getFullYear();
                var hours = msgDated.getHours();
                var min = msgDated.getMinutes();
                var hourString;
                var amPm = "am";
                if (hours > 11) {
                    amPm = "PM"
                    hourString = hours - 12;
                } else {
                    amPm = "AM"
                    hourString = hours;
                }
                if (parseInt(hourString) < 10)
                    hourString = '0' + hourString;
                if (parseInt(min) < 10)
                    min = '0' + min;
                var dipdate = day + ' ' + month + ', ' + year + ' at ' + hourString + ':' + min + ' ' + amPm;
                var $gravatarWrapper = $("<div/>").addClass("chat-gravatar-wrapper").appendTo($chatMessage);
                _this.$msgUserVar = $("<div/>").addClass("chat-message-user").appendTo($chatMessage);
                _this.$msgDateVar = $("<div/>").addClass("chat-message-date").appendTo($chatMessage);
                if (dipdate.indexOf('undefined') >= 0) {
                    dipdate = "";
                }
                _this.$msgDateVar.text(dipdate);
                _this.$msgUserVar.text(firstlastName);
                var $textWrapper = $("<div/>").addClass("chat-text-wrapper").appendTo($chatMessage);
                $messageP.appendTo($textWrapper);
                var messageUserFrom = _this.opts.chat.usersById[message.UserFromId];
                var RndColor = messageUserFrom.RandomColor;
                if (messageUserFrom.RandomColor == ' ' || messageUserFrom.RandomColor == " " || messageUserFrom.RandomColor == 'undefined' || messageUserFrom.RandomColor == '' || messageUserFrom.RandomColor == "" || messageUserFrom.RandomColor == null) {
                    RndColor = '#00bb8e'; // $("#imgPrfPhoto").css("background-color");
                }
                $("<img style='background: none repeat scroll 0 0 " + RndColor + ";padding-right: 0px;'/>").attr("src", (SiteUrl + messageUserFrom.ProfilePictureUrl)).appendTo($gravatarWrapper);
                _this.chatContainer.$windowInnerContent.scrollTop(_this.chatContainer.$windowInnerContent[0].scrollHeight);
            }
        };
        this.sendMessage = function (messageText) {
            var _this = this;
            var dipdate;
            var generateGuidPart = function () {
                return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
            };
            var clientGuid = (generateGuidPart() + generateGuidPart() + '-' + generateGuidPart() + '-' + generateGuidPart() + '-' + generateGuidPart() + '-' + generateGuidPart() + generateGuidPart() + generateGuidPart());
            _this.opts.adapter.server.sendMessage(_this.opts.otherUser.Id, messageText, clientGuid, function (messageHistory) {

            });
        };
        this.clearMsgHistory = function (myid, othersid) {            
            var _this = this;
            if (confirm("Are you sure? Do you want to clear message history?")) {
                _this.opts.adapter.server.clearMsgHistory(myid, othersid);
                _this.chatContainer.$windowInnerContent.text('');
                _this.chatContainer.$textBox.focus();

            }
        };
        this.sendTypingSignal = function () {
            var _this = this;
            _this.opts.adapter.server.sendTypingSignal(_this.opts.otherUser.Id);
        };
        this.getToggleState = function () {
            var _this = this;
            return _this.chatContainer.getToggleState();
        };
        this.setVisible = function (value) {
            var _this = this;
            _this.chatContainer.setVisible(value);
        };
    }
    ChatWindow.prototype = {
        init: function () {
            var _this = this;
            _this.chatContainer = $.chatContainer({
                title: _this.opts.userToName,
                canClose: true,
                initialToggleState: _this.opts.initialToggleState,
                onClose: function (e) {
                    _this.opts.onClose(e);
                },
                onToggleStateChanged: function (toggleState) {
                    _this.opts.onToggleStateChanged(toggleState);
                }
            });
            _this.chatContainer.$textBox.keypress(function (e) {
                if (_this.$sendTypingSignalTimeout == undefined) {
                    _this.$sendTypingSignalTimeout = setTimeout(function () {
                        _this.$sendTypingSignalTimeout = undefined;
                    }, 3000);
                    _this.sendTypingSignal();
                }
                if (e.which == 13) {
                    e.preventDefault();
                    if ($(this).val()) {
                        _this.sendMessage($(this).val());
                        $(this).val('').trigger("autosize.resize");
                        _this.chatContainer.$linkButton.css("display", "block");
                    }
                }
            });
            _this.chatContainer.setTitle(_this.opts.otherUser.Name);
            _this.opts.adapter.server.getMessageHistory(_this.opts.otherUser.Id, function (messageHistory) {
                if (messageHistory.length > 0) {
                    _this.chatContainer.$linkButton.css("display", "block");

                }
                else {
                    _this.chatContainer.$linkButton.css("display", "none");

                }
                var currentUserName = $('#spanFNLN').text();
                var otherUserName = _this.opts.otherUser.Name;
                for (var i = 0; i < messageHistory.length; i++) {
                    if (messageHistory[i].UserFromId == _this.opts.otherUser.Id) {
                        _this.addMessage(messageHistory[i], otherUserName);
                    }
                    else {
                        _this.addMessage(messageHistory[i], currentUserName);
                    }
                }
                _this.chatContainer.setVisible(true);
                if (_this.opts.initialFocusState == "focused")
                    _this.chatContainer.$textBox.focus();
                // scroll to the bottom
                _this.chatContainer.$windowInnerContent.scrollTop(_this.chatContainer.$windowInnerContent[0].scrollHeight);
                if (_this.opts.onReady)
                    _this.opts.onReady(_this);
            });
            _this.setOnlineStatus(_this.opts.userIsOnline);
            _this.chatContainer.$linkButton.click(function (e) {
                _this.clearMsgHistory(_this.opts.myUser.Id, _this.opts.otherUser.Id);
            });
        },
        focus: function () {
            var _this = this;
            _this.chatContainer.$textBox.focus();
        },
        showTypingSignal: function (user) {
            var _this = this;
            if (_this.$typingSignal)
                _this.$typingSignal.remove();
            _this.$typingSignal = $("<p/>").addClass("typing-signal").text(user.Name + _this.opts.typingText);
            _this.chatContainer.$windowInnerContent.after(_this.$typingSignal);
            if (_this.typingSignalTimeout)
                clearTimeout(_this.typingSignalTimeout);
            _this.typingSignalTimeout = setTimeout(function () {
                _this.removeTypingSignal();
            }, 5000);
        },
        removeTypingSignal: function () {
            var _this = this;
            if (_this.$typingSignal)
                _this.$typingSignal.remove();
            if (_this.typingSignalTimeout)
                clearTimeout(_this.typingSignalTimeout);
        },
        setOnlineStatus: function (userIsOnline) {
            var _this = this;
            _this.chatContainer.$textBox.attr("id", "txtChtUsr_" + _this.opts.otherUser.Id);
            if (userIsOnline) {
                _this.chatContainer.$windowTitle.addClass("online");
                _this.chatContainer.$OfflineMsg.hide();                
                _this.chatContainer.$windowTitle.removeClass("offline");
            } else {
                _this.chatContainer.$windowTitle.removeClass("online");
                var OfflineMessage = _this.chatContainer.$OfflineMsg.text();
                var UserName = _this.chatContainer.$windowTitle.text();
                OfflineMessage = OfflineMessage.replace("#UserName#", UserName);
                OfflineMessage = OfflineMessage.replace("#UserName#", UserName);
                _this.chatContainer.$OfflineMsg.text(OfflineMessage);
                _this.chatContainer.$OfflineMsg.show();
                _this.chatContainer.$windowTitle.addClass("offline");
            }
        }
    };
    $.chatWindow = function (options) {
        var chatWindow = new ChatWindow(options);
        chatWindow.init();
        return chatWindow;
    };
})(jQuery);

// CHAT
(function ($) {
    function Chat(options) {
        var _this = this;
        _this.defaults = {
            user: null,
            adapter: null,
            titleText: 'Chat',
            emptyRoomText: "There's no other users",
            typingText: " is typing...",
            useActivityIndicatorPlugin: true,
            playSound: true
        };
        //Extending options:
        _this.opts = $.extend({}, _this.defaults, options);
        //Privates:
        _this.$el = null;
        // there will be one property on this object for each user in the chat
        // the property FullName is the other user id (toStringed)
        _this.chatWindows = new Object();
        _this.lastMessageCheckTimeStamp = null;
        _this.chatContainer = null;
        _this.usersById = {};
    }
    // Separate functionality from object creation
    Chat.prototype = {
        init: function () {
            var _this = this;
            var mainChatWindowChatState = _this.readCookie("main_window_chat_state");
            if (!mainChatWindowChatState)
                mainChatWindowChatState = "maximized";
            // will create user list chat container
            _this.chatContainer = $.chatContainer({
                title: _this.opts.titleText,
                showTextBox: false,
                canClose: true,
                initialToggleState: mainChatWindowChatState,
                onCreated: function (container) {
                    if (!container.$windowInnerContent.html()) {
                        var $loadingBox = $("<div/>").addClass("loading-box").appendTo(container.$windowInnerContent);
                        if (_this.opts.useActivityIndicatorPlugin)
                            $loadingBox.activity({ segments: 8, width: 3, space: 0, length: 3, color: '#666666', speed: 1.5 });
                    }
                },
                onToggleStateChanged: function (toggleState) {
                    _this.createCookie("main_window_chat_state", toggleState);
                }
            });
            // the client functions are functions that must be called by the chat-adapter to interact
            // with the chat
            _this.client = {
                sendMessage: function (message, firstlastName) {
                    var tmpmessage = message.Message.split('-');
                    if (message.UserFromId != _this.opts.user.Id) {
                        if (!_this.chatWindows[message.UserFromId]) {
                            if (tmpmessage[0] != "RD") {
                                _this.createNewChatWindow(message.UserFromId);
                            }
                            else {
                                // Start R-Discussion
                                if (tmpmessage[2] != "Close" && tmpmessage[1] != undefined) {
                                    // Check R-Discussion Tab is active or not
                                    var TabSelectedID = $("#horizontalTab li.resp-tab-active").attr("id");
                                    if (TabSelectedID == "tabRDiscussion") {
                                        // Check Current Discussion or not                                                           
                                        if ($("#RDiscussionID").val() != undefined && (parseInt($("#RDiscussionID").val()) != 0 || parseInt($("#RDiscussionID").val()) != "NaN") && parseInt(tmpmessage[1]) != 0) {
                                            $('ul li').removeClass('active');
                                            if (parseInt($("#RDiscussionID").val()) == parseInt(tmpmessage[1])) {
                                                RefreshRDiscussion(false, null, null, false);
                                                GetDiscussionHistoryFromID(tmpmessage[1], $("#DiscussionType").val(), true);
                                                if ($("#DiscussionType").val() == "Other") {
                                                    $('#otherdiscussion').click();
                                                    RefreshRDiscussion(false, "TabOtherDiscussion", $("#RDiscussionID").val(), true);
                                                } else {
                                                    $('#importantdiscussion').click();
                                                    RefreshRDiscussion(false, "TabImportantDiscussion", $("#RDiscussionID").val(), true);
                                                }
                                                //DiscussionHighLightSelected($("#DiscussionList_" + $("#RDiscussionID").val())); 
                                                $("#li" + $("#RDiscussionID").val()).addClass('active');
                                            }
                                            else {
                                                // Other discussion code
                                                if ($("#DiscussionType").val() == "Other") {
                                                    GetDiscussionHistoryFromID($("#RDiscussionID").val(), 'Other', true);
                                                    RefreshRDiscussion(false, "TabOtherDiscussion", $("#RDiscussionID").val(), false);
                                                    var count = $("#DiscussionOtherTotal").html();
                                                    // $("#DiscussionOtherTotal").html(parseInt(parseInt(count)+1));    
                                                    $('#otherdiscussion').click();
                                                    //DiscussionHighLightSelected($("#DiscussionList_" + $("#RDiscussionID").val())); 
                                                    $("#li" + $("#RDiscussionID").val()).addClass('active');
                                                }
                                                else {
                                                    GetDiscussionHistoryFromID($("#RDiscussionID").val(), 'Important', true);
                                                    RefreshRDiscussion(false, "TabImportantDiscussion", $("#RDiscussionID").val(), false);
                                                    var count = $("#DiscussionImportantTotal").html();
                                                    $('#importantdiscussion').click();
                                                    $("#li" + $("#RDiscussionID").val()).addClass('active');
                                                }
                                                RDiscussionCount();
                                                var totalImpCount = $("#DiscussionImportantTotal").html().trim();
                                                var totalOtherCount = $("#DiscussionOtherTotal").html().trim();
                                                var RDiscussionTabcount = 0;
                                                if (totalImpCount != '' && totalImpCount != null && totalImpCount != 'NaN') {
                                                    RDiscussionTabcount += parseInt(totalImpCount);
                                                }
                                                if (totalOtherCount != '' && totalOtherCount != null && totalOtherCount != 'NaN') {
                                                    RDiscussionTabcount += parseInt(totalOtherCount);
                                                }
                                                $(".RDiscussionCount").html(RDiscussionTabcount);
                                            }

                                        }
                                        else if ($("#RDiscussionID").val() == undefined) {
                                            RDiscussionCount(); RefreshRDiscussion(true, null, null, true);
                                            GetDiscussionHistoryFromID($("#RDiscussionID").val(), $("#DiscussionType").val(), true);
                                            RefreshRDiscussion(false, "TabImportantDiscussion", $("#RDiscussionID").val(), false);
                                            var count = $("#DiscussionImportantTotal").html();
                                            $("#DiscussionImportantTotal").html(parseInt(parseInt(count) + 1));
                                            if ($("#DiscussionType").val() == "Important") {
                                                $('#importantdiscussion').click();
                                            } else {
                                                $('#otherdiscussion').click();
                                            }
                                            $("#li" + $("#RDiscussionID").val()).addClass('active');
                                        }
                                        else {
                                            if (parseInt(tmpmessage[1]) == 0) {
                                                if ($('#ActiveTabNameDiscussion').val() != "0") {
                                                    GetDiscussionHistoryFromID($("#RDiscussionID").val(), $("#DiscussionType").val(), true);
                                                    RefreshRDiscussion(false, "TabImportantDiscussion", $("#RDiscussionID").val(), false);
                                                    if ($("#DiscussionType").val() == "Important") {
                                                        $('#importantdiscussion').click();
                                                        RefreshRDiscussion(false, "TabImportantDiscussion", null, false);
                                                    }
                                                    else {
                                                        $('#otherdiscussion').click();
                                                        RefreshRDiscussion(false, "TabOtherDiscussion", null, false);
                                                    }
                                                    $("#li" + $("#RDiscussionID").val()).addClass('active');
                                                }
                                            }
                                        }
                                    }
                                    else {
                                        var totalImpCount = $("#DiscussionImportantTotal").html().trim();
                                        var totalOtherCount = $("#DiscussionOtherTotal").html().trim();
                                        var RDiscussionTabcount = 0;
                                        if (totalImpCount != '' && totalImpCount != null && totalImpCount != 'NaN') {
                                            RDiscussionTabcount += parseInt(totalImpCount);
                                        }
                                        if (totalOtherCount != '' && totalOtherCount != null && totalOtherCount != 'NaN') {
                                            RDiscussionTabcount += parseInt(totalOtherCount);
                                        }
                                        $(".RDiscussionCount").html(RDiscussionTabcount);
                                    }
                                }
                                else {
                                    RDiscussionCount(); RefreshRDiscussion(false, null, null, true);
                                }
                            }
                        }
                        else {
                            if (tmpmessage[0] != "RD") {
                                _this.chatWindows[message.UserFromId].addMessage(message, firstlastName);
                            }
                            else {
                                if (tmpmessage[2] != "Close" && tmpmessage[1] != undefined) {
                                    var TabSelectedID = $("#horizontalTab li.resp-tab-active").attr("id");
                                    if (TabSelectedID == "tabRDiscussion") {
                                        if ($("#RDiscussionID").val() != undefined && (parseInt($("#RDiscussionID").val()) != 0 || parseInt($("#RDiscussionID").val()) != "NaN") && parseInt(tmpmessage[1]) != 0) {
                                            $('ul li').removeClass('active');
                                            if (parseInt($("#RDiscussionID").val()) == parseInt(tmpmessage[1])) {
                                                GetDiscussionHistoryFromID(tmpmessage[1], null, true);
                                                if ($("#DiscussionType").val() == "Other") {
                                                    $('#otherdiscussion').click();
                                                } else {
                                                    $('#importantdiscussion').click();
                                                }
                                                $("#li" + $("#RDiscussionID").val()).addClass('active');
                                            }
                                            else {
                                                if ($("#DiscussionType").val() == "Other") {
                                                    GetDiscussionHistoryFromID($("#RDiscussionID").val(), 'Other', true);
                                                    RefreshRDiscussion(false, "TabOtherDiscussion", $("#RDiscussionID").val(), false);
                                                    var count = $("#DiscussionOtherTotal").html();
                                                    $('#otherdiscussion').click();
                                                    $("#li" + RDiscussionID).addClass('active');
                                                }
                                                else {
                                                    GetDiscussionHistoryFromID($("#RDiscussionID").val(), 'Important', true);
                                                    RefreshRDiscussion(false, "TabImportantDiscussion", $("#RDiscussionID").val(), false);
                                                    var count = $("#DiscussionImportantTotal").html();
                                                    $('#importantdiscussion').click();
                                                    $("#li" + $("#RDiscussionID").val()).addClass('active');
                                                }
                                                var totalImpCount = $("#DiscussionImportantTotal").html().trim();
                                                var totalOtherCount = $("#DiscussionOtherTotal").html().trim();
                                                var RDiscussionTabcount = 0;
                                                if (totalImpCount != '' && totalImpCount != null && totalImpCount != 'NaN') {
                                                    RDiscussionTabcount += parseInt(totalImpCount);
                                                }
                                                if (totalOtherCount != '' && totalOtherCount != null && totalOtherCount != 'NaN') {
                                                    RDiscussionTabcount += parseInt(totalOtherCount);
                                                }
                                                $(".RDiscussionCount").html(RDiscussionTabcount);
                                            }
                                        }
                                        else if ($("#RDiscussionID").val() == undefined) {
                                            RDiscussionCount(); RefreshRDiscussion(true, null, null, true);
                                            GetDiscussionHistoryFromID($("#RDiscussionID").val(), $("#DiscussionType").val(), true);
                                            RefreshRDiscussion(false, "TabImportantDiscussion", $("#RDiscussionID").val(), false);
                                            var count = $("#DiscussionImportantTotal").html();
                                            $("#DiscussionImportantTotal").html(parseInt(parseInt(count) + 1));
                                            if ($("#DiscussionType").val() == "Important") {
                                                $('#importantdiscussion').click();
                                            } else {
                                                $('#otherdiscussion').click();
                                            }
                                            $("#li" + $("#RDiscussionID").val()).addClass('active');
                                        }
                                        else {
                                            if (parseInt(tmpmessage[1]) == 0) {
                                                if ($('#ActiveTabNameDiscussion').val() != "0") {
                                                    GetDiscussionHistoryFromID($("#RDiscussionID").val(), $("#DiscussionType").val(), true);
                                                    RefreshRDiscussion(false, "TabImportantDiscussion", $("#RDiscussionID").val(), false);
                                                    if ($("#DiscussionType").val() == "Important") {
                                                        $('#importantdiscussion').click();
                                                        RefreshRDiscussion(false, "TabImportantDiscussion", null, false);
                                                    } else {
                                                        $('#otherdiscussion').click();
                                                        RefreshRDiscussion(false, "TabOtherDiscussion", null, false);
                                                    }
                                                    $("#li" + $("#RDiscussionID").val()).addClass('active');
                                                }
                                            }
                                        }
                                    }
                                    else {
                                        var totalImpCount = $("#DiscussionImportantTotal").html().trim();
                                        var totalOtherCount = $("#DiscussionOtherTotal").html().trim();
                                        var RDiscussionTabcount = 0;
                                        if (totalImpCount != '' && totalImpCount != null && totalImpCount != 'NaN') {
                                            RDiscussionTabcount += parseInt(totalImpCount);
                                        }
                                        if (totalOtherCount != '' && totalOtherCount != null && totalOtherCount != 'NaN') {
                                            RDiscussionTabcount += parseInt(totalOtherCount);
                                        }
                                        $(".RDiscussionCount").html(RDiscussionTabcount);
                                    }
                                }
                                else {
                                    RDiscussionCount(); RefreshRDiscussion(false, null, null, true);
                                }
                            }
                        }
                        if (_this.opts.playSound) {
                            if (message.Message != "system genertated msg for new discussion") {
                                var filePath = SiteUrl + "chatjs/Sounds/chat";
                                _this.playSound(filePath);
                            }
                        }
                    }
                    else {
                        if (_this.chatWindows[message.UserToId]) {
                            if (tmpmessage[0] != "RD") {
                                _this.chatWindows[message.UserToId].addMessage(message, firstlastName);
                            }
                            else {
                                alert("Same user");
                            }
                        }
                        else {
                        }

                        if (message.UserFromId == _this.opts.user.Id) {
                            if (tmpmessage[0] == "RD" && tmpmessage[2] == "Close" && tmpmessage[1] != undefined) {
                                var TabSelectedID = $("#horizontalTab li.resp-tab-active").attr("id");
                                if (TabSelectedID == "tabRDiscussion") {
                                    if ($("#RDiscussionID").val() != undefined && (parseInt($("#RDiscussionID").val()) != 0 || parseInt($("#RDiscussionID").val()) != "NaN") && parseInt(tmpmessage[1]) != 0) {
                                        $('ul li').removeClass('active');
                                        if (parseInt($("#RDiscussionID").val()) == parseInt(tmpmessage[1])) {
                                            RefreshRDiscussion(false, null, null, false);
                                            GetDiscussionHistoryFromID(tmpmessage[1], $("#DiscussionType").val(), true);
                                            if ($("#DiscussionType").val() == "Other") {
                                                $('#otherdiscussion').click();
                                                RefreshRDiscussion(false, "TabOtherDiscussion", $("#RDiscussionID").val(), true);
                                            } else {
                                                $('#importantdiscussion').click();
                                                RefreshRDiscussion(false, "TabImportantDiscussion", $("#RDiscussionID").val(), true);
                                            }
                                            $("#li" + $("#RDiscussionID").val()).addClass('active');
                                        }
                                        else {
                                            if ($("#DiscussionType").val() == "Other") {
                                                GetDiscussionHistoryFromID($("#RDiscussionID").val(), 'Other', true);
                                                RefreshRDiscussion(false, "TabOtherDiscussion", $("#RDiscussionID").val(), false);
                                                var count = $("#DiscussionOtherTotal").html();
                                                $('#otherdiscussion').click();
                                                $("#li" + $("#RDiscussionID").val()).addClass('active');

                                            }
                                            else {
                                                GetDiscussionHistoryFromID($("#RDiscussionID").val(), 'Important', true);
                                                RefreshRDiscussion(false, "TabImportantDiscussion", $("#RDiscussionID").val(), false);
                                                var count = $("#DiscussionImportantTotal").html();
                                                $('#importantdiscussion').click();
                                                $("#li" + $("#RDiscussionID").val()).addClass('active');
                                            }
                                            RDiscussionCount();
                                            var totalImpCount = $("#DiscussionImportantTotal").html().trim();
                                            var totalOtherCount = $("#DiscussionOtherTotal").html().trim();
                                            var RDiscussionTabcount = 0;
                                            if (totalImpCount != '' && totalImpCount != null && totalImpCount != 'NaN') {
                                                RDiscussionTabcount += parseInt(totalImpCount);
                                            }
                                            if (totalOtherCount != '' && totalOtherCount != null && totalOtherCount != 'NaN') {
                                                RDiscussionTabcount += parseInt(totalOtherCount);
                                            }
                                            $(".RDiscussionCount").html(RDiscussionTabcount);
                                        }
                                    }
                                    else if ($("#RDiscussionID").val() == undefined) {
                                        RDiscussionCount(); RefreshRDiscussion(true, null, null, true);
                                        GetDiscussionHistoryFromID($("#RDiscussionID").val(), $("#DiscussionType").val(), true);
                                        RefreshRDiscussion(false, "TabImportantDiscussion", $("#RDiscussionID").val(), false);
                                        var count = $("#DiscussionImportantTotal").html();
                                        $("#DiscussionImportantTotal").html(parseInt(parseInt(count) + 1));
                                        if ($("#DiscussionType").val() == "Important") {
                                            $('#importantdiscussion').click();
                                        } else {
                                            $('#otherdiscussion').click();
                                        }
                                        $("#li" + $("#RDiscussionID").val()).addClass('active');
                                    }
                                    else {
                                        if (parseInt(tmpmessage[1]) == 0) {
                                            if ($('#ActiveTabNameDiscussion').val() != "0") {
                                                GetDiscussionHistoryFromID($("#RDiscussionID").val(), $("#DiscussionType").val(), true);
                                                RefreshRDiscussion(false, "TabImportantDiscussion", $("#RDiscussionID").val(), false);
                                                if ($("#DiscussionType").val() == "Important") {
                                                    $('#importantdiscussion').click();
                                                    RefreshRDiscussion(false, "TabImportantDiscussion", null, false);
                                                }
                                                else {
                                                    $('#otherdiscussion').click();
                                                    RefreshRDiscussion(false, "TabOtherDiscussion", null, false);
                                                }
                                                $("#li" + $("#RDiscussionID").val()).addClass('active');
                                            }
                                        }
                                    }
                                }
                                else {
                                    var totalImpCount = "";
                                    if ($("#DiscussionImportantTotal").length > 0) {
                                        totalImpCount = $("#DiscussionImportantTotal").html().trim();
                                    }
                                    var totalOtherCount = "";
                                    if ($("#DiscussionOtherTotal").length > 0) {
                                        totalOtherCount = $("#DiscussionOtherTotal").html().trim();
                                    }
                                    var RDiscussionTabcount = 0;
                                    if (totalImpCount != '' && totalImpCount != null && totalImpCount != 'NaN') {
                                        RDiscussionTabcount += parseInt(totalImpCount);
                                    }
                                    if (totalOtherCount != '' && totalOtherCount != null && totalOtherCount != 'NaN') {
                                        RDiscussionTabcount += parseInt(totalOtherCount);
                                    }
                                    if ($(".RDiscussionCount").length > 0) {
                                        $(".RDiscussionCount").html(RDiscussionTabcount);
                                    }
                                }
                            }                            
                        }
                    }
                },
                clearMsgHistory: function (myid, othersid) {                
                },
                sendTypingSignal: function (otherUserId) {
                    /// <summary>Called by the adapter when the OTHER user is sending a typing signal to the current user</summary>
                    /// <param FullName="otherUser" type="Object">User object (the other sending the typing signal)</param>
                    if (_this.chatWindows[otherUserId]) {
                        var otherUser = _this.usersById[otherUserId];
                        _this.chatWindows[otherUserId].showTypingSignal(otherUser);
                    }
                },
                msgOfflineList: function (OfflineMsgUsers) {
                    //------ Open  Window for Offline Meesages -----------------------
                    if (OfflineMsgUsers.length != 0) {
                        for (var i = 0; i < OfflineMsgUsers.length; i++) {
                            var idx = OfflineMsgUsers[i]
                            _this.createNewChatWindow(idx);
                        }
                    }                    
                },
                usersListChanged: function (usersList) {
                    /// <summary>Called by the adapter when the users list changes</summary>
                    /// <param FullName="usersList" type="Object">The new user list</param>                    
                    // initializes the user list with the current user, because he/she will not be retrieved
                    _this.usersById = {};
                    _this.usersById[_this.opts.user.Id] = _this.opts.user;
                    _this.chatContainer.getContent().html('');                     
                    if (usersList.length == 0) {
                        $("<div/>").addClass("user-list-empty").text(_this.opts.emptyRoomText).appendTo(_this.chatContainer.getContent());
                    }
                    else {
                        var $onlineList = $("<div/>").appendTo(_this.chatContainer.getContent());
                        var $offlineList = $("<div/>").appendTo(_this.chatContainer.getContent());
                        var $onlineDiv = $("<div/>").appendTo($onlineList).attr("style", 'padding-top: 10px; padding-left: 10px; padding-bottom: 5px;');
                        var $onlineImage = $("<img style='margin-left:5px;'/>").attr("src", SiteUrl + 'Scripts/Common/ChatJs/Images/chat-online.png').appendTo($onlineDiv);
                        var $onlineTitle = $("<span/>").attr("id", "divOnline").text("Users (0 / 0)").addClass("onlineofflinetitle").appendTo($onlineDiv);

                        var $offlineTitle = $("<span/>").attr("id", "divOffline").appendTo($offlineList);
                        var onlineCounter = 0;
                        var offnlineCounter = 0;
                        for (var i = 0; i < usersList.length; i++) {
                            if (usersList[i].Id != _this.opts.user.Id) {
                                _this.usersById[usersList[i].Id] = usersList[i];
                                var UDetailsForTitle = (usersList[i].Email != null ? 'Email : ' + usersList[i].Email : '') + '\n' + (usersList[i].Mobile != null ? 'Mobile : ' + usersList[i].Mobile : '');
                                var $userListItem = $("<div/>")
                                    .addClass("user-list-item")
                                    .attr("data-val-id", usersList[i].Id)
                                    .attr("id", "chtusrlst_" + usersList[i].Id)
                                    //.attr("title", UDetailsForTitle);
                                if (usersList[i].Status == 0) {
                                    $userListItem.appendTo($offlineList);
                                    offnlineCounter++;
                                }
                                else {
                                    $userListItem.appendTo($onlineList);
                                    onlineCounter++;

                                }                                
                                $("<img style='background: none repeat scroll 0 0 " + usersList[i].RandomColor + ";padding-right: 0px;'/>")
                                    .addClass("profile-picture")
                                    .attr("src", (SiteUrl + usersList[i].ProfilePictureUrl))
                                    .appendTo($userListItem);

                                $("<div/>")
                                    .addClass("profile-status")
                                    .addClass(usersList[i].Status == 0 ? "offline" : "online")
                                    .appendTo($userListItem);

                                $("<div/>")
                                    .addClass("content")
                                    .text(usersList[i].Name)
                                    .appendTo($userListItem);

                                // makes a click in the user to either create a new chat window or open an existing
                                // I must clusure the 'i'
                                (function (otherUserId) {
                                    // handles clicking in a user. Starts up a new chat session
                                    $userListItem.click(function () {
                                        if (_this.chatWindows[otherUserId]) {
                                            _this.chatWindows[otherUserId].focus();
                                        } else
                                            _this.createNewChatWindow(otherUserId);
                                    });
                                })(usersList[i].Id);
                            }
                        }
                        $onlineTitle.text("Users  (" + onlineCounter + " / " + (onlineCounter + offnlineCounter) + ")");
                    }
                    // update the online status of the remaining windows
                    for (var i in _this.chatWindows) {
                        if (_this.usersById && _this.usersById[i])
                            _this.chatWindows[i].setOnlineStatus(_this.usersById[i].Status == 1);
                        else
                            _this.chatWindows[i].setOnlineStatus(false);
                    }
                    _this.chatContainer.setVisible(true);
                },
                showError: function (errorMessage) {                  
                }
            };
            _this.opts.adapter.init(_this, function () {
                // gets the user list
                _this.opts.adapter.server.getUsersList(function (usersList) {
                    _this.client.usersListChanged(usersList);
                    // Load Last time Opened window
                });
                _this.opts.adapter.server.getOffUsersMsgList(function (OfflineMsgUsers) {
                    _this.client.msgOfflineList(OfflineMsgUsers);
                    // Load Last time Opened window
                });
            });
        },
        playSound: function (filename) {
            /// <summary>Plays a notification sound</summary>
            /// <param FullName="fileFullName" type="String">The file path without extension</param>
            var $soundContainer = $("#soundContainer");
            if (!$soundContainer.length)
                $soundContainer = $("<div>").attr("id", "soundContainer").appendTo($("body"));
            $soundContainer.html('<audio autoplay="autoplay"><source src="' + filename + '.mp3" type="audio/mpeg" /><source src="' + filename + '.ogg" type="audio/ogg" /><embed hidden="true" autostart="true" loop="false" src="' + filename + '.wav" /></audio>');
        },
        loadWindows: function () {
            var _this = this;
            var cookie = _this.readCookie("chat_state");
            if (cookie) {
                var openedChatWindows = JSON.parse(cookie);
                for (var i = 0; i < openedChatWindows.length; i++) {
                    var otherUserId = openedChatWindows[i].userId;
                    _this.opts.adapter.server.getUserInfo(otherUserId, function (user) {
                        if (user) {
                            if (!_this.chatWindows[otherUserId])
                                _this.createNewChatWindow(otherUserId, null, "blured");
                        } else {
                            // when an error occur, the state of this cookie invalid
                            // it must be destroyed
                            _this.eraseCookie("chat_state");
                        }
                    });
                }
            }
        },
        saveWindows: function () {
            var _this = this;
            var openedChatWindows = new Array();
            for (var otherUserId in _this.chatWindows) {
                openedChatWindows.push({
                    userId: otherUserId,
                    toggleState: _this.chatWindows[otherUserId].getToggleState()
                });
            }
            _this.createCookie("chat_state", JSON.stringify(openedChatWindows), 365);
        },
        createNewChatWindow: function (otherUserId, initialToggleState, initialFocusState) {
            if (!initialToggleState)
                initialToggleState = "maximized";
            if (!initialFocusState)
                initialFocusState = "focused";
            var _this = this;
            var otherUser = _this.usersById[otherUserId];
            if (!otherUser)
                throw "Cannot find the other user in the list";
            var newChatWindow = $.chatWindow({
                chat: _this,
                myUser: _this.opts.user,
                gender: _this.opts.gender,
                otherUser: otherUser,
                newMessageUrl: _this.opts.newMessageUrl,
                messageHistoryUrl: _this.opts.messageHistoryUrl,
                initialToggleState: initialToggleState,
                initialFocusState: initialFocusState,
                userIsOnline: otherUser.Status == 1,
                adapter: _this.opts.adapter,
                typingText: _this.opts.typingText,
                onClose: function () {
                    delete _this.chatWindows[otherUser.Id];
                    $.organizeChatContainers();
                    _this.saveWindows();
                },
                onToggleStateChanged: function (toggleState) {
                    _this.saveWindows();
                }
            });
            // this cannot be in t
            _this.chatWindows[otherUser.Id.toString()] = newChatWindow;
            _this.saveWindows();
        },
        eraseCookie: function (name) {
            var _this = this;
            _this.createCookie(name, "", -1);
        },
        readCookie: function (name) {
            var nameEq = name + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') c = c.substring(1, c.length);
                if (c.indexOf(nameEq) == 0) return c.substring(nameEq.length, c.length);
            }
            return null;
        },
        createCookie: function (name, value, days) {
            var expires;
            if (days) {
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                expires = "; expires=" + date.toGMTString();
            } else {
                expires = "";
            }
            document.cookie = name + "=" + value + expires + "; path=/";
        }
    };
    // The actual plugin
    $.chat = function (options) {
        var chat = new Chat(options);
        chat.init();
        return chat;
    };
})(jQuery);

function OpenInstantChatBox() {
    //$("#instantMsgTxt").val("");
    //document.getElementById("divErrorInsMsg").style.display = 'none';
    //var UsersHTML = "";
    //$.ajax({
    //    type: "POST",
    //    url: SiteUrl + "Home/GetUsersListForInstantMessage",
    //    data: {},
    //    cache: false,
    //    async: false,
    //    success: function (data) {
    //        UsersHTML = data.UsersHTML;
    //    }
    //});
    //$("#divIMUsers").html(UsersHTML);
    //$("#divInstantMessage").modal('show');
    //$("#divInstantMessage").on('shown', function () {
    //    if (navigator.userAgent.match(/iPad/i)) {
    //    }
    //    else {
    //        $('#instantMsgTxt', this).focus();
    //    }
    //});

    $("#instantMsgTxt").val("");
    document.getElementById("divErrorInsMsg").style.display = 'none';
    var UsersHTML = "";
    $.ajax({
        type: "POST",
        url: SiteUrl + "Home/GetUsersList",
        data: {},
        cache: false,
        async: false,
        success: function (data) {
            UsersHTML = data.UsersHTML;
        }
    });
    $("#divIMUsers").html(UsersHTML);
    $("#instantUserID").select2();
    $("#divInstantMessage").modal('show');
    $("#divInstantMessage").on('shown', function () {
        if (navigator.userAgent.match(/iPad/i)) {
        }
        else {
            $('#instantMsgTxt', this).focus();
        }
    });
}

function SendInstantMsgToUsers() {   
    var strErrMsg = "";
    var isValid = true;
    var total_selected = 0;
    if ($("#instantUserID").val() != null && $("#instantUserID").val().length > 0)
    {
        total_selected = $("#instantUserID").val().length;
    }
    var allVals = [];
    if (total_selected > 0) {
        $('#hdnSelectedUserIDs').val($("#instantUserID").val());
    }
    else {
        strErrMsg += 'Please select at least one user.';
        isValid = false;
    }
    if (jQuery.trim(document.getElementById('instantMsgTxt').value).length == 0) {
        if (strErrMsg.length > 0)
            strErrMsg += "<br />";
        strErrMsg += 'Please enter instant message.';
        isValid = false;
    }
    if (isValid == false) {
        document.getElementById('divErrorInsMsg').style.display = '';
        document.getElementById('divErrorInsMsg').innerHTML = strErrMsg;
        return false;
    }
    else {
        document.getElementById('divErrorInsMsg').style.display = 'none';
        $("#frmInstantMessageToUsers").ajaxSubmit({
            target: "",
            type: "POST",
            data: { SelectedUserIDs: $('#hdnSelectedUserIDs').val(), InstantMessage: jQuery.trim(document.getElementById('instantMsgTxt').value) },
            success: function (res) {                
                var obj = jQuery.parseJSON(res);
                if (obj.Status == "0") {
                    document.getElementById('divErrorInsMsg').style.display = '';
                    document.getElementById('divErrorInsMsg').innerHTML = 'Instant message was not sent.';                    
                    window.scrollTo(0, 0);
                    return false;
                }
                else if (obj.Status == "1") { 
                    document.getElementById('divErrorInsMsg').style.display = '';
                    document.getElementById('divErrorInsMsg').innerHTML = 'Instant message send successfully to selected users.';
                    $("#divInstantMessage").modal('hide');
                    alert("Instant message send successfully to selected users.");
                    return false;
                }
                else if (obj.Status == "-2") { // Server Side Exception
                    document.getElementById('divErrorInsMsg').style.display = '';
                    document.getElementById('divErrorInsMsg').innerHTML = 'There was an error, Please contact your administrator.';                    
                    return false;
                }
                else if (obj.Status == "-3") { // Session Expired
                    window.location.reload(true);
                }
                $(".modal-body").each(function () {
                    this.scrollTop = 0;
                });
            }
        });
        return true;
    }
}

function SetChatUserChkbox() {
    if ($("#chkChatSelAll").prop('checked')) {
        $("input[name=cbIMUser]").prop('checked', 'checked');
    }
    else {
        $("input[name=cbIMUser]").prop('checked', false);
    }
}