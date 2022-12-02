var viewer = new CLIPBOARD_CLASS("render_viewer");
var d_placeholder = document.getElementById("drop_placeholder");
var img_frames = {
    /**
     * <frame_id (only by import order)>:
     * {
     *    source: <blob:url>,
     *    name_info: <info from "parse_filename()">,
     *    img_size: {  },
     * }
     */
};
var editing_target = {
    dat: undefined, // "menu/person.dat"
    hash: undefined,
    ext: undefined,
};
var viewing_idx = undefined;
var offset_base = { dx: 0, dy: 0 };
window.canvas_sz = {};
window._ani_playing = false;

//#region Object extension functions
Object.loop = function (inp, callback) {
    Object.keys(inp).some(function (k) {
        if (callback(k, inp[k]) === false) return true; // terminate loop
    });
};
Object.isExist = function (inp, nest_key_name, value) {
    var target = false;
    Object.loop(inp, function (k, v) {
        if (Array.isArray(nest_key_name)) {
            var x = v;
            nest_key_name.forEach((inner_v) => {
                // dereference
                x = x[inner_v];
            });
            if (x == value) {
                target = k;
                return false; // terminate loop
            }
        } else {
            if (v[nest_key_name] == value) {
                target = k;
                return false; // terminate
            }
        }
    });
    return target;
};
Object.sz = function (inp) {
    return Object.keys(inp).length;
};
Object.keyAtIdx = function (inp, idx) {
    return Object.keys(inp)[idx];
};
Object.findKeyByValue = function (inp, v) {
    let keys = Object.keys(inp);
    keys.forEach(e => {
        return e == v;
    });
};
Object.findKeyIndex = function (inp, v) {
    return Object.keys(inp).findIndex(e => e == v);
};
(() => {
    // @credit: stackoverflow.com/a/66998406
    // overrides URL methods to be able to retrieve the original blobs later on
    const old_create = URL.createObjectURL;
    const old_revoke = URL.revokeObjectURL;
    Object.defineProperty(URL, "createObjectURL", {
        get: () => storeAndCreate,
    });
    Object.defineProperty(URL, "revokeObjectURL", {
        get: () => forgetAndRevoke,
    });
    Object.defineProperty(URL, "getFromObjectURL", {
        get: () => getBlob,
    });
    const dict = {};

    function storeAndCreate(blob) {
        const url = old_create(blob); // let it throw if it has to
        dict[url] = blob;
        return url;
    }

    function forgetAndRevoke(url) {
        old_revoke(url);
        try {
            if (new URL(url).protocol === "blob:") {
                delete dict[url];
            }
        } catch (e) {}
    }

    function getBlob(url) {
        return dict[url] || null;
    }
})();
//#endregion

function checkKey(e) {
    e = e || window.event;
    let has_ctl = e.ctrlKey,
        has_sft = e.shiftKey,
        has_alt = e.altKey;
    // e.location == 0 -> general key, 1 -> left-side, 2 -> right-side, 3 -> numpad

    if (has_sft) {
        if (e.code == "ArrowUp") {
            $(`#btn_ani_off_up`).click()
        } else if (e.code == "ArrowDown") {
            $(`#btn_ani_off_down`).click()
        } else if (e.code == "ArrowLeft") {
            $(`#btn_ani_off_left`).click()
        } else if (e.code == "ArrowRight") {
            $(`#btn_ani_off_right`).click()
        }else
        {
            handled = false
        }

        if (handled) {
            e.stopPropagation();
            e.preventDefault();
        }
        return;
    }
    if (has_ctl)
    {
        let handled = true;
        if (e.code == "KeyR") {
            if (has_sft) {
                return;
            }
            $(`#btn_reset`).click();
        } else if (e.code == "Space") {
            $(`#btn_ani_play`).click();
        } else if (e.code == "KeyQ") {
            $(`#btn_update_dat`).click();
        } else {
            handled = false;
        }

        if (handled)
        {
            e.stopPropagation();
            e.preventDefault();
        }
        return;
    }

    if (e.code == "ArrowUp") {
        // up arrow (frame 0)
        let target = Object.keys(img_frames)[0];
        viewing_idx = target;
        viewer.updateCanvas(img_frames[target]);
        showImgInfo();
    } else if (e.code == "ArrowDown") {
        // down arrow (last frame)
        let target = Object.keys(img_frames)[Object.sz(img_frames) - 1];
        viewing_idx = target;
        viewer.updateCanvas(img_frames[target]);
        showImgInfo();
    } else if (e.code == "ArrowLeft") {
        // left arrow
        idx = parseInt(Object.keys(img_frames).find((e) => e == "" + viewing_idx));
        max = Math.max(0, idx - 1);
        kai = Object.keyAtIdx(img_frames, max);
        let target = parseInt(kai);
        viewing_idx = target;
        viewer.updateCanvas(img_frames[target]);
        showImgInfo();
    } else if (e.code == "ArrowRight") {
        // right arrow
        idx = parseInt(Object.keys(img_frames).find((e) => e == "" + viewing_idx));
        min = Math.min(idx + 1, Object.sz(img_frames) - 1);
        kai = Object.keyAtIdx(img_frames, min);
        let target = parseInt(kai);
        viewing_idx = target;
        viewer.updateCanvas(img_frames[target]);
        showImgInfo();
    }
    else
        return;
    
    e.preventDefault();
}

// animation delay setting
window._ani = {
    // fps: 12 fps
    fps: 11,
    // delay (n)ms before restart from frame 0
    rst: 0,
};
function loop_ani()
{
    var ori_delay = 1000 / window._ani.fps;
    startAni();
    
    function startAni() {
        var previous = 0;
        var current_frame = -1;
        var delay = ori_delay;
        requestAnimationFrame(animation);
        
        function animation(now) {
            if (window._ani_playing === false)
                return; // stop ani
            
            requestAnimationFrame(animation);
            
            if (now - previous >= delay) {
                if (delay != ori_delay)
                {
                    // repeat
                    current_frame = -1;
                    delay = ori_delay;
                }
                current_frame++;
                
                if (current_frame >= window.ordered_keys.length) {
                    delay = window._ani.rst;
                } else {
                    // draw
                    viewing_idx = window.ordered_keys[current_frame];
                    viewer.updateCanvas(img_frames[viewing_idx]);
                    showImgInfo();
                }

                previous = now;
            }
        }
    }
}

function showImgInfo() {
    let me =
        viewing_idx !== undefined
            ? img_frames[viewing_idx]
            : {
                  img_size: {
                      w: 0,
                      h: 0,
                  },
                  name_info: {
                      raw: undefined,
                  },
              };
    $(`#file_px`).text(
        `Size(w,h)[${me.img_size.w.toString().padStart(4, "0")}×${me.img_size.h.toString().padStart(4, "0")}px]`
    );
    if (me.name_info.raw !== undefined) {
        $(`#file_name`).text(`Name[${me.name_info.raw}]`);
        $(`#file_dat`).text(me.name_info.folders.join("/") + "/" + me.name_info.dat_name + ".dat");
        $(`#file_hash`).text("0x" + me.name_info.hash.toString(16).toUpperCase());
    } else {
        $(`#file_name`).text(`Name[----]`);
        $(`#file_dat`).text("----");
        $(`#file_hash`).text("--------");
    }
}

function new_frame(img_data_src, name_info) {
    let k = Object.isExist(img_frames, ["name_info", "raw"], name_info.raw);
    if (k !== false) {
        // replace
        index = k;
    } else {
        // new
        let sz = Object.sz(img_frames);
        if (sz == 0) {
            // no existing
            index = 0;
        } else {
            // has existing, use the idx of it + 1
            index = parseInt(Object.keyAtIdx(img_frames, sz - 1)) + 1;
        }
        $("#sortable").append(`<div class="ordered_frames" data-id="${index}"></div>`);
        // init
        img_frames[index] = {};
    }

    img_frames[index].source = img_data_src;
    img_frames[index].name_info = name_info;

    $(`.ordered_frames[data-id="${index}"]`).css({
        "background-image": `url(${img_data_src})`,
        "background-size": "100px 100px",
        "background-repeat": "no-repeat",
    });

    // bind click listener
    $(".ordered_frames")
        .off("click")
        .on("click", function (e) {
            let self = e.target;
            let target = $(self).data("id");
            // edit that
            viewing_idx = target;
            viewer.updateCanvas(img_frames[target]);
            showImgInfo();
        });

    $(".ordered_frames")
        .off("mouseup")
        .on("mouseup", function (e) {
            if (e.which == 2) {
                // since "click" event wont be triggered by "middle btn", so, use "mouseup" instead.
                // middle btn -> remove
                let self = e.target;
                let target = $(self).data("id");

                e.preventDefault();

                // go to next existing img at same pos (self_idx)
                idx = parseInt(Object.findKeyIndex(img_frames, target));

                // remove from dict
                delete img_frames[target];
                // remove from ordered_keys list if exist
                window.ordered_keys = window.ordered_keys.filter(e => e != target);

                min = Math.min(idx, Object.sz(img_frames) - 1);
                kai = Object.keyAtIdx(img_frames, min);
                
                if (Object.sz(img_frames) > 0) {
                    viewing_idx = kai;
                    // redraw
                    viewer.updateCanvas(img_frames[viewing_idx]);
                    showImgInfo();
                } else {
                    // no more existing img, call reset
                    $(`#btn_reset`).click();
                }


                $(e.target).off("mouseup");
                // remove self
                $(self).remove();
            }
        });

    // return index of the frame
    return index;
}

// sample: menu___main__3eb7a049_file092_map--heropos.tca.tca_f0_off5x_5y.png
const regex_filename = /(\w+___)*([\w_]+)__([0-9a-f]{8})_file[\d]+(_[\w_\-\.]+)*\.(tc[abp])_f(\d+)_off([\-\d]+)x_([\-\d]+)y\.png/i;
// sample: $b0001$00$06$01.tcp_fSh_76_off27x_82y.png
// sample: $abc$test$name.tcp_f0_0_off-1x_2y.png
const regex_filename_dollar = /([\w\$_\-\.]+)*\.(tc[abp])_f(SW|Wt|NW|Nt|Sh|0)_(\d+)_off([\-\d]+)x_([\-\d]+)y\.png/i;
function parse_filename(str) {
    isDollar = str.indexOf("$") > -1;

    let matched = (isDollar ? regex_filename_dollar : regex_filename).exec(str);
    if (matched === null || matched.length != (isDollar ? 7 : 9)) {
        alert(`Invalid file name\nInput[${str}]`);
        return false;
    }

    let folders = (matched[1] || (isDollar ? "$" : "___")).split(isDollar ? "$" : "___");
    file_name = folders.pop();
    if (isDollar)
    {
        return {
            folders: folders,
            dat_name: file_name,
            hash: "FFFFFFFF",
            ext: matched[2],
            frame_index: parseInt(matched[4]),
            off_x: parseInt(matched[5]),
            off_y: parseInt(matched[6]),
            raw: str,
        };
    }

    return {
        folders: folders,
        dat_name: matched[2],
        hash: matched[3],
        ext: matched[5],
        frame_index: parseInt(matched[6]),
        off_x: parseInt(matched[7]),
        off_y: parseInt(matched[8]),
        raw: str,
    };
}

function get_frame_idx_from_name(str)
{
    return parseInt(str.split(/tc[abp]_f\w?\w?_?/g, 3)[1].split("_", 1)[0]);
}

function sortObjectByKeys(o) {
    // @credit: https://thewebdev.info/2021/02/20/how-to-sort-javascript-object-property-by-values/
    return (
        Object.keys(o)
            .sort((a, b) => {
                // order by file name
                if (b.indexOf(a.split(/\.tc[abp]/, 1)[0]) > -1) {
                    // same name
                    fa = get_frame_idx_from_name(a);
                    fb = get_frame_idx_from_name(b);
                    return fa - fb;
                }

                return a.localeCompare(b);
            })
            .reduce(
                (obj, key) => ({
                    ...obj,
                    [key]: o[key],
                }),
                {}
            )
    );
}

function inverse(obj) {
    var retobj = {};
    for (var key in obj) {
        retobj[obj[key]] = key;
    }
    return retobj;
}

/**
 * image pasting into canvas
 *
 * @param {string} canvas_id - canvas id
 * @param {boolean} autoresize - if canvas will be resized
 * @src https://stackoverflow.com/questions/54182727/drag-and-drop-image-convert-to-base64
 * @src http://jsfiddle.net/wrv369/cbqe39L5/
 */
function CLIPBOARD_CLASS(canvas_id) {
    var _self = this;
    var ctx = document.getElementById(canvas_id).getContext("2d");
    const canvas = (_self.canvas = document.getElementById(canvas_id));
    _self.offset_base = {};

    //#region handlers
    document.addEventListener(
        "dragover",
        function (e) {
            e.preventDefault();
        },
        false
    );
    document.addEventListener("drop", function (e) {
        // prevent default action (open as link for some elements)
        // add event handler to canvas if desired instead of document
        e.preventDefault();
        var items = Array.from(e.dataTransfer.items).filter((ele) => ele.type.indexOf("image") > -1);
        let target_ele = e.toElement.id;
        var is_replace = false;
        if (["render_viewer", "viewer_container"].includes(target_ele)) {
            // drop to "viewer" area
            if (items.length > 1) {
                alert(
                    `You can only use one image file for replacement.\nIf you want to add new frames, drop to the blue area below!`
                );
                return;
            }
            is_replace = true;
        } else if (target_ele == "sortable") {
            // drop to "sortable" area
        } else if (target_ele == "img_ref") {
            // drop to "img_ref" area

            if (Object.sz(img_frames) <= 0)
            {
                alert(`Please drop a valid image file to the editor area first!`);
                return;
            }

            var blob = items[0].getAsFile();
            var source = URL.createObjectURL(blob);

            // take target's info as new info

            let _current_info = {};
            // clone
            Object.assign(_current_info, img_frames[viewing_idx]);
            // update the source ofcuz
            _current_info.source = source;
            // update background
            $(`#img_ref`).css({
                "background-image": `url("${source}")`,
            });
            $(`#viewer_container`).css({
                "background-image": `url("${source}")`,
                "background-position": `${_self.offset_base.dx - _current_info.off_x}px ${_self.offset_base.dy - _current_info.off_y}px`,
            });

            return;
        } else {
            // not in drop area
            return;
        }

        // sort order by file name
        let names = {};
        for (var i = 0; i < items.length; i++) {
            names[e.dataTransfer.files[i].name] = i;
        }
        names = sortObjectByKeys(names); // sorted
        names_keys = Object.keys(names);

        d_placeholder.style.visibility = "hidden";
        let error_shown = false;
        for (var x = 0; x < items.length; x++) {
            // check is same hash
            let fn = names_keys[x];
            let i = names[fn];

            if (is_replace && Object.sz(img_frames) >= 1)
            {
                // replace mode if at least one
                var blob = items[i].getAsFile();
                var source = URL.createObjectURL(blob);

                // take target's info as new info
                let _current_info = img_frames[viewing_idx];
                // update the source ofcuz
                _current_info.source = source;
                // size might have changes, so, let's update it
                _self.updateCanvas(_current_info, { idx: new_frame(source, _current_info.name_info) }, function (callback_info) {
                    let idx = callback_info.idx;
                    // update frame info
                    img_frames[idx].img_size = {
                        w: callback_info.pastedImage.width,
                        h: callback_info.pastedImage.height,
                    };
                    // update viewing_idx
                    viewing_idx = idx;
                    // display frame info
                    showImgInfo();
                });
                return;
            }

            let info = parse_filename(fn);
            if (info === false) {
                // failed to parse
                return;
            }
            
            if (!window._bypass && (editing_target.ext || info.ext).toLowerCase() == "tcb" && Object.sz(img_frames) >= 1) {
                // do not process next frame, since only 1 frame can be stored for TCB.
                alert(`Only 1 frame can be stored in TCB file!\nSkipping rest of the images`);
                break;
            }

            let ndat = info.folders.join("/") + "/" + info.dat_name + ".dat"; // ndat = new dat
            if (!window._bypass)
            {
                if (editing_target.dat !== undefined && editing_target.hash !== undefined) {
                    if (ndat != editing_target.dat || info.hash != editing_target.hash) {
                        if (!error_shown)
                        {
                            alert(
                                `Fail to drop image!\nNew image is not same as the current editing target. Please save the changes first!\nCurrent[${editing_target.dat}] Hash[${editing_target.hash}]\nNew[${ndat}] Hash[${info.hash}] Raw[${info.raw}]`
                            );
                            error_shown = true;
                        }
                        continue; // break loop
                    }
                }
            }

            // image
            var blob = items[i].getAsFile();
            var source = URL.createObjectURL(blob);

            var idx = new_frame(source, info);
            img_frames[idx].off_x = info.off_x;
            img_frames[idx].off_y = info.off_y;

            // update canvas size by info from json
            let { w, h, dx, dy } = window.canvas_sz[info.raw.split("_file")[0]] || {
                w: 800,
                h: 640,
                dx: 400,
                dy: 320
            };
            canvas.width = parseInt(w);
            canvas.height = parseInt(h);
            _self.offset_base = { dx: parseInt(dx), dy: parseInt(dy) };

            // set file ext
            editing_target.ext = info.ext;
            editing_target.dat = ndat;
            editing_target.hash = info.hash;

            let img_info = img_frames[idx];
            _self.updateCanvas(img_info, { idx: idx }, function (callback_info) {
                let idx = callback_info.idx;
                // update frame info
                img_frames[idx].img_size = {
                    w: callback_info.pastedImage.width,
                    h: callback_info.pastedImage.height,
                };
                // update viewing_idx
                viewing_idx = idx;
                // display frame info
                showImgInfo();
            });
        }
    });
    //#endregion

    // draw on canvas
    this.updateCanvas = function (img_info, callback_info, callback) {
        var pastedImage = new Image();
        pastedImage.onload = function () {
            //clear canvas
            let [ox, oy] = [img_info.off_x, img_info.off_y];
            $(`#off_current`).text(`Current Offset: ${ox}x ${oy}y`);
            _self.clearCanvas(0, 0, canvas.width, canvas.height);
            ctx.drawImage(
                pastedImage,
                _self.offset_base.dx - ox,
                _self.offset_base.dy - oy,
                pastedImage.width,
                pastedImage.height
            );
            callback && callback(Object.assign(callback_info, { pastedImage: pastedImage }));
        };
        pastedImage.src = img_info.source;
    };

    // redraw canvas
    this.updateCurrent = function () {
        var img_info = img_frames[viewing_idx];
        var pastedImage = new Image();
        pastedImage.onload = function () {
            //clear canvas
            let [ox, oy] = [img_info.off_x, img_info.off_y];
            $(`#off_current`).text(`Current Offset: ${ox}x ${oy}y`);
            _self.clearCanvas(0, 0, canvas.width, canvas.height);
            ctx.drawImage(
                pastedImage,
                _self.offset_base.dx - ox,
                _self.offset_base.dy - oy,
                pastedImage.width,
                pastedImage.height
            );
        };
        pastedImage.src = img_info.source;
    };

    this.clearCanvas = function (x, y, w, h) {
        ctx.clearRect(x, y, w, h);
    };
}

// detect blank canvas: https://stackoverflow.com/a/17386803/177416
function isCanvasBlank(canvas) {
    var blank = document.createElement("canvas");
    blank.width = canvas.width;
    blank.height = canvas.height;

    return canvas.toDataURL() === blank.toDataURL();
}

function saveBlob(url, fileName) {
    // @credit: https://gist.github.com/philipstanislaus/c7de1f43b52531001412#gistcomment-3395184
    var a = document.createElement("a");
    document.body.appendChild(a);
    a.style = "display: none";
    a.id = "a_dl_" + Date.now();
    a.href = url;
    a.download = fileName;
    a.click();
    document.getElementById(a.id).remove();
};

function async_frame_to_base64(frame_key) {
    return new Promise((res, rej) => {
        try
        {
            fetch(img_frames[frame_key].source)
                .then((resp) => resp.blob())
                .then(r => {
                    var reader = new FileReader();
                    reader.readAsDataURL(r);
                    reader.onloadend = function () {
                        let target = img_frames[frame_key];
                        res({
                            data: reader.result,
                            offset: [target.off_x, target.off_y],
                        });
                    };
                });
        } catch (e) {
            console.error("rej", e)
            rej(e);
            // alert(`[F2B64] Failure, please see console for error details.`);
        }
    });
}

//#region Core Logic
$(function () {
    $("#sortable").sortable({
        placeholder: "ui-state-highlight",
        scrollSpeed: 5,
        tolerance: "pointer",
        revert: 300,
        opacity: 0.4,
        //axis: "x",
        containment: "parent",
    });
    $("#sortable").disableSelection();

    document.onkeydown = checkKey;

    // init.
    showImgInfo();
    $(`#off_current`).text(`Current Offset: 0x 0y`);

    // load canvas sz json
    $.getJSON("canvas.json", (data) => {
        window.canvas_sz = data;
    });

    $("#btn_reset").click(function () {
        window.location.reload();
        return;
        // revoke blob before empty
        // Object.values(img_frames).forEach(v => {
        //     URL.revokeObjectURL(v.source)
        // });

        // img_frames = {};
        // editing_target = {
        //     dat: undefined,
        //     hash: undefined,
        //     ext: undefined,
        // };
        // viewing_idx = undefined;
        // viewer.clearCanvas();
        // $(`#sortable *`).remove();
        // showImgInfo();
    });

    window.__is_requesting = false
    $(`#btn_update_dat`).click(function () {
        if (window.__is_requesting)
        {
            // block multi-req.
            return;
        }

        window.__is_requesting = !0;
        $(`#overlay`).toggleClass("hide");
        async_imgs = []; // list of promises
        $(`.ordered_frames[data-id]`).each((_, e) => {
            async_imgs.push(
                async_frame_to_base64(
                    $(e).data("id")
                )
            );
        });
        
        Promise.allSettled(async_imgs).then(rs => {
            var imgs = [];
            rs.forEach(res => {
                r = res.value;
                r.data = r.data.replace("data:image/png;base64,", "");
                imgs.push(r);
            });
            
            $.ajax({
                url: `/process`,
                method: "post",
                dataType: "json",
                contentType: "application/json;charset=utf-8",
                data: JSON.stringify({
                    dat: editing_target.dat,
                    hash: editing_target.hash,
                    ext: editing_target.ext,
                    canvas: {
                        size: [viewer.canvas.width, viewer.canvas.height],
                        base: [viewer.offset_base.dx, viewer.offset_base.dy],
                    },
                    imgs: imgs,
                }),
                success: (retn) => {
                    console.log(retn);
                    alert(`${retn}`);
                },
                error: (retn) => {
                    console.error(retn);
                    alert(`Failure: ${retn.statusText}`);
                },
                complete: (_) => {
                    $(`#overlay`).toggleClass("hide");
                    window.__is_requesting = !1;
                },
            });
        });
        
    });

    $("#btn_to_local").click(function () {
        $(`.ordered_frames[data-id]`).each((i, e) => {
            let frame = img_frames[$(e).data("id")];
            // update frame index by loop order
            let fn = frame.name_info.raw.replace(/_f(\d+)_off/, `_f${i}_off`);
            saveBlob(frame.source, fn);
        });
    });

    $("#btn_ani_play").click(function () {
        if (!window._ani_playing)
        {
            window._ani_playing = true;

            var ordered = [];
            $(`.ordered_frames[data-id]`).each((i, e) => {
                ordered.push($(e).data("id"));
            });
            window.ordered_keys = ordered;
            loop_ani();
        }
        else
            window._ani_playing = false;
        
    });

    // offset control
    $("#btn_ani_off_up").click(function () {
        img_frames[viewing_idx].off_y += 1;
        viewer.updateCurrent();
    });
    $("#btn_ani_off_down").click(function () {
        img_frames[viewing_idx].off_y -= 1;
        viewer.updateCurrent();
    });
    $("#btn_ani_off_left").click(function () {
        img_frames[viewing_idx].off_x += 1;
        viewer.updateCurrent();
    });
    $("#btn_ani_off_right").click(function () {
        img_frames[viewing_idx].off_x -= 1;
        viewer.updateCurrent();
    });
    $("#btn_ani_off_base").click(function () {
        img_frames[viewing_idx].off_x = img_frames[viewing_idx].name_info.off_x;
        img_frames[viewing_idx].off_y = img_frames[viewing_idx].name_info.off_y;
        viewer.updateCurrent();
    });
    $("#btn_show_hide_canvas").click(function () {
        $(`#render_viewer`).toggle();
    });
    $("#btn_show_hide_ref").click(function () {
        $(`#viewer_container`).toggleClass("force_bg");
    });
    
    $(`#img_ref`).on("mouseup", function (e) {
        if (e.which == 2) {
            e.preventDefault();
            $(`#img_ref`).css({
                "background-image": `none`,
            });
            $(`#viewer_container`).css({
                "background-image": `none`
            });
        }
    });
});
//#endregion
