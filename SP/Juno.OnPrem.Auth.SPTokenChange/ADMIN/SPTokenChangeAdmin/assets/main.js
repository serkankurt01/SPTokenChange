$(function () {
    $(".saveBtn").click(function () {

        var webId = $(this).attr("data-webid");
        var au = GetValue(webId, "txtAudiance").val();

        var configCacheItem = {
            item: {
                "WebId": webId,
                "Audiance": GetValue(webId, "txtAudiance").val(),
                "AuthType": GetValue(webId, "slcAuthType").val(),
                "CertificatePassword": GetValue(webId, "txtCertificatePassword").val(),
                "CertificatePath": GetValue(webId, "txtCertificatePath").val(),
                "Issuer": GetValue(webId, "txtIssuer").val(),
                "LifeTimeCheck": GetValue(webId, "chkLifeTimeCheck").is(':checked'),
                "DebugMode": GetValue(webId, "chkDebugMode").is(':checked'),
                "IsActive": GetValue(webId, "chkIsActive").is(':checked'),
                "SPClientId": GetValue(webId, "txtSPClientId").val(),
                "SPIssuerId": GetValue(webId, "txtSPIssuerId").val(),
                "SPRealm": GetValue(webId, "txtSPRealm").val(),
                "StsDiscoveryUrl": GetValue(webId, "txtSTSDiscoveryUrl").val(),
                "UPAuthType": GetValue(webId, "slcUPType").val(),
                "AuthProviderName": GetValue(webId, "txtProviderName").val(),
            }
        }

        $.ajax({
            type: "POST",
            url: "/_admin/sptokenchangeadmin/configcache.aspx/SaveConfig",
            data: JSON.stringify(configCacheItem),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                window.location.reload();
            },
            "headers": {
                "X-RequestDigest": $('#__REQUESTDIGEST').val(),
                "accept": "application/json",
                "content-type": "application/json",
            },

            error: function (r) {
                alert(r.responseText);
                console.log("AJAX error in request: " + JSON.stringify(r, null, 2));
            },
            failure: function (r) {
                alert(r.responseText);
            }
        });

    })


    $(".removeBtn").click(function () {

        var webId = $(this).attr("data-webid");


        $.ajax({
            type: "GET",
            url: "/_admin/sptokenchangeadmin/configcache.aspx?ops=Remove&webId=" + webId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                $("#sph" + webId).parent().remove()
            },
            "headers": {
                "X-RequestDigest": $('#__REQUESTDIGEST').val(),
                "accept": "application/json",
                "content-type": "application/json",
            },

            error: function (r) {
                $("#sph" + webId).parent().remove()
            },
            failure: function (r) {
                alert(r.responseText);
            }
        });

    })

    $('.desc').popover({
        trigger: 'focus'
    })

    function GetValue(webId, key) {
        return $("#sp" + webId + " ." + key);
    }

});