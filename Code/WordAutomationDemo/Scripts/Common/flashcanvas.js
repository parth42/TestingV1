window.ActiveXObject&&!window.CanvasRenderingContext2D&&function(n,t){function k(n){this.code=n;this.message=lt[n]}function at(n){this.width=n}function d(n){this.id=n.C++}function s(n){this.G=n;this.id=n.C++}function h(n,t){this.canvas=n;this.B=t;this.d=n.uniqueID;this.D();this.C=0;this.t="";var i=this;setInterval(function(){f[i.d]===0&&i.e()},30)}function p(){if(t.readyState==="complete"){t.detachEvent(ht,p);for(var i=t.getElementsByTagName(e),n=0,r=i.length;n<r;++n)y.initElement(i[n])}}function g(){var n=event.srcElement,t=n.parentNode;n.blur();t.focus()}function nt(){var n=event.propertyName;if(n==="width"||n==="height"){var t=event.srcElement,r=t[n],i=parseInt(r,10);(isNaN(i)||i<0)&&(i=n==="width"?300:150);r===i?(t.style[n]=i+"px",t.getContext("2d").I(t.width,t.height)):t[n]=i}}function tt(){var f,t,r,i;n.detachEvent(ct,tt);for(f in o){t=o[f];r=t.firstChild;for(i in r)typeof r[i]=="function"&&(r[i]=u);for(i in t)typeof t[i]=="function"&&(t[i]=u);r.detachEvent(ot,g);t.detachEvent(st,nt)}n[rt]=u;n[ut]=u;n[ft]=u;n[w]=u;n[et]=u}function vt(){var n=t.getElementsByTagName("script");return n=n[n.length-1],t.documentMode>=8?n.src:n.getAttribute("src",4)}function c(n){return(""+n).replace(/&/g,"&amp;").replace(/</g,"&lt;")}function yt(n){return n.toLowerCase()}function r(n){throw new k(n);}function it(n){var t=parseInt(n.width,10),i=parseInt(n.height,10);(isNaN(t)||t<0)&&(t=300);(isNaN(i)||i<0)&&(i=150);n.width=t;n.height=i}var u=null,e="canvas",rt="CanvasRenderingContext2D",ut="CanvasGradient",ft="CanvasPattern",w="FlashCanvas",et="G_vmlCanvasManager",ot="onfocus",st="onpropertychange",ht="onreadystatechange",ct="onunload",l=((n[w+"Options"]||{}).swfPath||vt().replace(/[^\/]+$/,""))+"flashcanvas.swf",i=new function(n){for(var t=0,i=n.length;t<i;t++)this[n[t]]=t}(["toDataURL","save","restore","scale","rotate","translate","transform","setTransform","globalAlpha","globalCompositeOperation","strokeStyle","fillStyle","createLinearGradient","createRadialGradient","createPattern","lineWidth","lineCap","lineJoin","miterLimit","shadowOffsetX","shadowOffsetY","shadowBlur","shadowColor","clearRect","fillRect","strokeRect","beginPath","closePath","moveTo","lineTo","quadraticCurveTo","bezierCurveTo","arcTo","rect","arc","fill","stroke","clip","isPointInPath","font","textAlign","textBaseline","fillText","strokeText","measureText","drawImage","createImageData","getImageData","putImageData","addColorStop","direction","resize"]),a={},f={},o={},v={},lt,y,b;h.prototype={save:function(){this.b();this.c();this.m();this.l();this.z();this.w();this.F.push([this.f,this.g,this.A,this.u,this.j,this.h,this.i,this.k,this.p,this.q,this.n,this.o,this.v,this.r,this.s]);this.a.push(i.save)},restore:function(){var n=this.F;n.length&&(n=n.pop(),this.globalAlpha=n[0],this.globalCompositeOperation=n[1],this.strokeStyle=n[2],this.fillStyle=n[3],this.lineWidth=n[4],this.lineCap=n[5],this.lineJoin=n[6],this.miterLimit=n[7],this.shadowOffsetX=n[8],this.shadowOffsetY=n[9],this.shadowBlur=n[10],this.shadowColor=n[11],this.font=n[12],this.textAlign=n[13],this.textBaseline=n[14]);this.a.push(i.restore)},scale:function(n,t){this.a.push(i.scale,n,t)},rotate:function(n){this.a.push(i.rotate,n)},translate:function(n,t){this.a.push(i.translate,n,t)},transform:function(n,t,r,u,f,e){this.a.push(i.transform,n,t,r,u,f,e)},setTransform:function(n,t,r,u,f,e){this.a.push(i.setTransform,n,t,r,u,f,e)},b:function(){var n=this.a;this.f!==this.globalAlpha&&(this.f=this.globalAlpha,n.push(i.globalAlpha,this.f));this.g!==this.globalCompositeOperation&&(this.g=this.globalCompositeOperation,n.push(i.globalCompositeOperation,this.g))},m:function(){if(this.A!==this.strokeStyle){var n=this.A=this.strokeStyle;this.a.push(i.strokeStyle,typeof n=="object"?n.id:n)}},l:function(){if(this.u!==this.fillStyle){var n=this.u=this.fillStyle;this.a.push(i.fillStyle,typeof n=="object"?n.id:n)}},createLinearGradient:function(n,t,u,f){return isFinite(n)&&isFinite(t)&&isFinite(u)&&isFinite(f)||r(9),this.a.push(i.createLinearGradient,n,t,u,f),new s(this)},createRadialGradient:function(n,t,u,f,e,o){return isFinite(n)&&isFinite(t)&&isFinite(u)&&isFinite(f)&&isFinite(e)&&isFinite(o)||r(9),(u<0||o<0)&&r(1),this.a.push(i.createRadialGradient,n,t,u,f,e,o),new s(this)},createPattern:function(n,t){n||r(17);var o=n.tagName,s,h=this.d;if(o)if(o=o.toLowerCase(),o==="img")s=n.getAttribute("src",2);else{if(o===e||o==="video")return;r(17)}else n.src?s=n.src:r(17);return t==="repeat"||t==="no-repeat"||t==="repeat-x"||t==="repeat-y"||t===""||t===u||r(12),this.a.push(i.createPattern,c(s),t),a[h]&&(this.e(),++f[h]),new d(this)},z:function(){var n=this.a;this.j!==this.lineWidth&&(this.j=this.lineWidth,n.push(i.lineWidth,this.j));this.h!==this.lineCap&&(this.h=this.lineCap,n.push(i.lineCap,this.h));this.i!==this.lineJoin&&(this.i=this.lineJoin,n.push(i.lineJoin,this.i));this.k!==this.miterLimit&&(this.k=this.miterLimit,n.push(i.miterLimit,this.k))},c:function(){var n=this.a;this.p!==this.shadowOffsetX&&(this.p=this.shadowOffsetX,n.push(i.shadowOffsetX,this.p));this.q!==this.shadowOffsetY&&(this.q=this.shadowOffsetY,n.push(i.shadowOffsetY,this.q));this.n!==this.shadowBlur&&(this.n=this.shadowBlur,n.push(i.shadowBlur,this.n));this.o!==this.shadowColor&&(this.o=this.shadowColor,n.push(i.shadowColor,this.o))},clearRect:function(n,t,r,u){this.a.push(i.clearRect,n,t,r,u)},fillRect:function(n,t,r,u){this.b();this.c();this.l();this.a.push(i.fillRect,n,t,r,u)},strokeRect:function(n,t,r,u){this.b();this.c();this.m();this.z();this.a.push(i.strokeRect,n,t,r,u)},beginPath:function(){this.a.push(i.beginPath)},closePath:function(){this.a.push(i.closePath)},moveTo:function(n,t){this.a.push(i.moveTo,n,t)},lineTo:function(n,t){this.a.push(i.lineTo,n,t)},quadraticCurveTo:function(n,t,r,u){this.a.push(i.quadraticCurveTo,n,t,r,u)},bezierCurveTo:function(n,t,r,u,f,e){this.a.push(i.bezierCurveTo,n,t,r,u,f,e)},arcTo:function(n,t,u,f,e){e<0&&isFinite(e)&&r(1);this.a.push(i.arcTo,n,t,u,f,e)},rect:function(n,t,r,u){this.a.push(i.rect,n,t,r,u)},arc:function(n,t,u,f,e,o){u<0&&isFinite(u)&&r(1);this.a.push(i.arc,n,t,u,f,e,o?1:0)},fill:function(){this.b();this.c();this.l();this.a.push(i.fill)},stroke:function(){this.b();this.c();this.m();this.z();this.a.push(i.stroke)},clip:function(){this.a.push(i.clip)},w:function(){var n=this.a,t,r;if(this.v!==this.font)try{t=v[this.d];t.style.font=this.v=this.font;r=t.currentStyle;n.push(i.font,[r.fontStyle,r.fontWeight,t.offsetHeight,r.fontFamily].join(" "))}catch(u){}this.r!==this.textAlign&&(this.r=this.textAlign,n.push(i.textAlign,this.r));this.s!==this.textBaseline&&(this.s=this.textBaseline,n.push(i.textBaseline,this.s));this.t!==this.canvas.currentStyle.direction&&(this.t=this.canvas.currentStyle.direction,n.push(i.direction,this.t))},fillText:function(n,t,r,u){this.b();this.l();this.c();this.w();this.a.push(i.fillText,c(n),t,r,u===void 0?Infinity:u)},strokeText:function(n,t,r,u){this.b();this.m();this.c();this.w();this.a.push(i.strokeText,c(n),t,r,u===void 0?Infinity:u)},measureText:function(n){var t=v[this.d];try{t.style.font=this.font}catch(i){}return t.innerText=n.replace(/[ \n\f\r]/g,"\t"),new at(t.offsetWidth)},drawImage:function(n,t,u,o,s,h,l,v,y){n||r(17);var w=n.tagName,p,b=arguments.length,k=this.d;if(w)if(w=w.toLowerCase(),w==="img")p=n.getAttribute("src",2);else{if(w===e||w==="video")return;r(17)}else n.src?p=n.src:r(17);if(this.b(),this.c(),p=c(p),b===3)this.a.push(i.drawImage,b,p,t,u);else if(b===5)this.a.push(i.drawImage,b,p,t,u,o,s);else if(b===9)(o===0||s===0)&&r(1),this.a.push(i.drawImage,b,p,t,u,o,s,h,l,v,y);else return;a[k]&&(this.e(),++f[k])},D:function(){this.globalAlpha=this.f=1;this.globalCompositeOperation=this.g="source-over";this.fillStyle=this.u=this.strokeStyle=this.A="#000000";this.lineWidth=this.j=1;this.lineCap=this.h="butt";this.lineJoin=this.i="miter";this.miterLimit=this.k=10;this.shadowBlur=this.n=this.shadowOffsetY=this.q=this.shadowOffsetX=this.p=0;this.shadowColor=this.o="rgba(0, 0, 0, 0.0)";this.font=this.v="10px sans-serif";this.textAlign=this.r="start";this.textBaseline=this.s="alphabetic";this.a=[];this.F=[]},H:function(){var n=this.a;return this.a=[],n},e:function(){var n=this.H();if(n.length>0)return eval(this.B.CallFunction('<invoke name="executeCommand" returntype="javascript"><arguments><string>'+n.join("&#0;")+"<\/string><\/arguments><\/invoke>"))},I:function(n,t){this.e();this.D();n>0&&(this.B.width=n);t>0&&(this.B.height=t);this.a.push(i.resize,n,t)}};s.prototype={addColorStop:function(n,t){(isNaN(n)||n<0||n>1)&&r(1);this.G.a.push(i.addColorStop,this.id,n,t)}};k.prototype=Error();lt={1:"INDEX_SIZE_ERR",9:"NOT_SUPPORTED_ERR",11:"INVALID_STATE_ERR",12:"SYNTAX_ERR",17:"TYPE_MISMATCH_ERR",18:"SECURITY_ERR"};y={initElement:function(r){var e,y,s,p,w,c;return r.getContext?r:(e=r.uniqueID,y="external"+e,a[e]=!1,f[e]=1,it(r),r.innerHTML='<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="'+location.protocol+'//fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0" width="100%" height="100%" id="'+y+'"><param name="allowScriptAccess" value="always"><param name="flashvars" value="id='+y+'"><param name="wmode" value="transparent"><\/object><span style="margin:0;padding:0;border:0;display:inline-block;position:static;height:1em;overflow:visible;white-space:nowrap"><\/span>',o[e]=r,s=r.firstChild,v[e]=r.lastChild,p=t.body.contains,p(r)?s.movie=l:w=setInterval(function(){p(r)&&(clearInterval(w),s.movie=l)},0),t.compatMode!=="BackCompat"&&n.XMLHttpRequest||(v[e].style.overflow="hidden"),c=new h(r,s),r.getContext=function(n){return n==="2d"?c:u},r.toDataURL=function(n,t){return(""+n).replace(/[A-Z]+/g,yt)==="image/jpeg"?c.a.push(i.toDataURL,n,typeof t=="number"?t:""):c.a.push(i.toDataURL,n),c.e()},s.attachEvent(ot,g),r)},saveImage:function(n){n.firstChild.saveImage()},setOptions:function(){},trigger:function(n,t){o[n].fireEvent("on"+t)},unlock:function(n,t){if(f[n]&&--f[n],t){var i=o[n],e=i.firstChild,r,u;it(i);r=i.width;u=i.height;i.style.width=r+"px";i.style.height=u+"px";r>0&&(e.width=r);u>0&&(e.height=u);e.resize(r,u);i.attachEvent(st,nt);a[n]=!0}}};t.createElement(e);t.createStyleSheet().cssText=e+"{display:inline-block;overflow:hidden;width:300px;height:150px}";t.readyState==="complete"?p():t.attachEvent(ht,p);n.attachEvent(ct,tt);l.indexOf(location.protocol+"//"+location.host+"/")===0&&(b=new ActiveXObject("Microsoft.XMLHTTP"),b.open("GET",l,!1),b.send(u));n[rt]=h;n[ut]=s;n[ft]=d;n[w]=y;n[et]={init:function(){},init_:function(){},initElement:y.initElement};keep=h.measureText}(window,document)