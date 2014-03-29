The following command is used to compile the document, taking care of references, bibliography and figure in 'eps' format:

latex repport.tex && bibtex repport && latex repport.tex && dvipdfm repport.dvi
