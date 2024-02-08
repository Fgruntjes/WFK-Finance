import DOMPurify from "dompurify";
import React, { HTMLAttributes } from "react";

type SafeHtmlProps<T extends HTMLElement> = HTMLAttributes<T> & {
  html: string;
  elementType?: keyof HTMLElementTagNameMap;
};

function SafeHtml<T extends HTMLElement = HTMLElement>({
  html,
  elementType = "div",
  ...elementProps
}: SafeHtmlProps<T>) {
  const cleanHTML = DOMPurify.sanitize(html, { ALLOWED_TAGS: ["br"] });

  return React.createElement(elementType, {
    dangerouslySetInnerHTML: { __html: cleanHTML },
    ...elementProps,
  });
}

export default SafeHtml;
