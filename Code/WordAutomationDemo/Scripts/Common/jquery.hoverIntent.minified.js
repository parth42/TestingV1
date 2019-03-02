/*!
 * hoverIntent r7 // 2013.03.11 // jQuery 1.9.1+
 * http://cherne.net/brian/resources/jquery.hoverIntent.html
 *
 * You may use hoverIntent under the terms of the MIT license.
 * Copyright 2007, 2013 Brian Cherne
 */
(function(n){n.fn.hoverIntent=function(t,i,r){var u={interval:100,sensitivity:7,timeout:0};u=typeof t=="object"?n.extend(u,t):n.isFunction(i)?n.extend(u,{over:t,out:i,selector:r}):n.extend(u,{over:t,out:t,selector:i});var f,e,o,s,h=function(n){f=n.pageX;e=n.pageY},c=function(t,i){if(i.hoverIntent_t=clearTimeout(i.hoverIntent_t),Math.abs(o-f)+Math.abs(s-e)<u.sensitivity)return n(i).off("mousemove.hoverIntent",h),i.hoverIntent_s=1,u.over.apply(i,[t]);o=f;s=e;i.hoverIntent_t=setTimeout(function(){c(t,i)},u.interval)},a=function(n,t){return t.hoverIntent_t=clearTimeout(t.hoverIntent_t),t.hoverIntent_s=0,u.out.apply(t,[n])},l=function(t){var r=jQuery.extend({},t),i=this;if(i.hoverIntent_t&&(i.hoverIntent_t=clearTimeout(i.hoverIntent_t)),t.type=="mouseenter"){o=r.pageX;s=r.pageY;n(i).on("mousemove.hoverIntent",h);i.hoverIntent_s!=1&&(i.hoverIntent_t=setTimeout(function(){c(r,i)},u.interval))}else n(i).off("mousemove.hoverIntent",h),i.hoverIntent_s==1&&(i.hoverIntent_t=setTimeout(function(){a(r,i)},u.timeout))};return this.on({"mouseenter.hoverIntent":l,"mouseleave.hoverIntent":l},u.selector)}})(jQuery)